// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Microsoft.Extensions.Logging.AzureWebAppDiagnostics.Internal
{
    /// <summary>
    /// The <see cref="ILogEventSink"/> implemenation that stores messages by appending them to Azure Blob in batches.
    /// </summary>
    public class AzureBlobSink : PeriodicBatchingSink
    {
        private const string BlobPathSeparator = "/";

        private readonly string _appName;
        private readonly string _fileName;
        private readonly ITextFormatter _formatter;
        private readonly CloudBlobContainer _container;

        /// <summary>
        /// Creates a new instance of <see cref="AzureBlobSink"/>
        /// </summary>
        /// <param name="container">The container to store logs to.</param>
        /// <param name="appName">The application name to use in blob path generation.</param>
        /// <param name="fileName">The last segment of blob name.</param>
        /// <param name="formatter">The <see cref="ITextFormatter"/> for log messages.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        public AzureBlobSink(CloudBlobContainer container,
            string appName,
            string fileName,
            ITextFormatter formatter,
            int batchSizeLimit,
            TimeSpan period) : base(batchSizeLimit, period)
        {
            _appName = appName;
            _fileName = fileName;
            _formatter = formatter;
            if (batchSizeLimit < 1)
            {
                throw new ArgumentException(nameof(batchSizeLimit));
            }
            _container = container;
        }

        /// <inheritdoc />
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var eventGroups = events.GroupBy(GetBlobKey);
            foreach (var eventGroup in eventGroups)
            {
                var blobName = string.Concat(
                    _appName, BlobPathSeparator,
                    eventGroup.Key.Item1, BlobPathSeparator,
                    eventGroup.Key.Item2, BlobPathSeparator,
                    eventGroup.Key.Item3, BlobPathSeparator,
                    eventGroup.Key.Item4, BlobPathSeparator,
                    _fileName
                );

                var blob = _container.GetAppendBlobReference(blobName);

                CloudBlobStream stream;
                try
                {
                    stream = await blob.OpenWriteAsync(createNew: false);
                }
                // Blob does not exist
                catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 404)
                {
                    await blob.CreateOrReplaceAsync(AccessCondition.GenerateIfNotExistsCondition(), null, null);
                    stream = await blob.OpenWriteAsync(createNew: false);
                }

                using (stream)
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var logEvent in eventGroup)
                        {
                            _formatter.Format(logEvent, writer);
                        }
                    }
                }
            }
        }

        private Tuple<int,int,int,int> GetBlobKey(LogEvent e)
        {
            return Tuple.Create(e.Timestamp.Year,
                e.Timestamp.Month,
                e.Timestamp.Day,
                e.Timestamp.Hour);
        }
    }
}