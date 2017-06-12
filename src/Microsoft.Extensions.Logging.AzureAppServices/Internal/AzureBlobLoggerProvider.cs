// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    /// <summary>
    /// The <see cref="ILoggerProvider"/> implementation that stores messages by appending them to Azure Blob in batches.
    /// </summary>
    [ProviderAlias("AzureAppServicesBlob")]
    public class AzureBlobLoggerProvider : BatchingLoggerProvider
    {
        private readonly string _appName;
        private readonly string _fileName;
        private readonly Func<string, ICloudAppendBlob> _blobReferenceFactory;

        /// <summary>
        /// Creates a new instance of <see cref="AzureBlobLoggerProvider"/>
        /// </summary>
        /// <param name="options"></param>
        internal AzureBlobLoggerProvider(IOptions<AzureDiagnosticsBlobLoggerOptions> options)
            : this(options,
                   GetDefaultBlobReferenceFactory(options))
        {
        }

        private static Func<string, ICloudAppendBlob> GetDefaultBlobReferenceFactory(IOptions<AzureDiagnosticsBlobLoggerOptions> options)
        {
            var container = new CloudBlobContainer(new Uri(options.Value.ContainerUrl));
            return name => new BlobAppendReferenceWrapper(container.GetAppendBlobReference(name));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AzureBlobLoggerProvider"/>
        /// </summary>
        /// <param name="blobReferenceFactory">The container to store logs to.</param>
        /// <param name="options"></param>
        internal AzureBlobLoggerProvider(
            IOptions<AzureDiagnosticsBlobLoggerOptions> options,
            Func<string, ICloudAppendBlob> blobReferenceFactory) :
            base(options)
        {
            var value = options.Value;
            _appName = value.ApplicationName;
            _fileName = value.ApplicationInstanceId + "_" + value.BlobName;
            _blobReferenceFactory = blobReferenceFactory;
        }

        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages)
        {
            var eventGroups = messages.GroupBy(GetBlobKey);
            foreach (var eventGroup in eventGroups)
            {
                var key = eventGroup.Key;
                var blobName = $"{_appName}/{key.Year}/{key.Month:00}/{key.Day:00}/{key.Hour:00}/{_fileName}";

                var blob = _blobReferenceFactory(blobName);

                Stream stream;
                try
                {
                    stream = await blob.OpenWriteAsync();
                }
                // Blob does not exist
                catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 404)
                {
                    await blob.CreateAsync();
                    stream = await blob.OpenWriteAsync();
                }

                using (var writer = new StreamWriter(stream))
                {
                    foreach (var logEvent in eventGroup)
                    {
                        writer.Write(logEvent.Message);
                    }
                }
            }
        }

        private (int Year, int Month, int Day, int Hour) GetBlobKey(LogMessage e)
        {
            return (e.Timestamp.Year,
                e.Timestamp.Month,
                e.Timestamp.Day,
                e.Timestamp.Hour);
        }
    }
}