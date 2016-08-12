// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Display;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Extensions.Logging.AzureWebAppDiagnostics.Internal
{
    /// <summary>
    /// The <see cref="SerilogLoggerProvider"/> implemenation that creates instances of <see cref="Serilog.Core.Logger"/> connected to <see cref="AzureBlobSink"/>.
    /// </summary>
    public class AzureBlobLoggerProvider : SerilogLoggerProvider
    {
        private readonly string _outputTemplate;
        private readonly string _appName;
        private readonly string _fileName;
        private readonly int _batchSize;
        private readonly TimeSpan _period;

        /// <summary>
        /// Creates a new instance of the <see cref="AzureBlobLoggerProvider"/> class.
        /// </summary>
        /// <param name="outputTemplate"></param>
        /// <param name="appName"></param>
        /// <param name="fileName"></param>
        /// <param name="batchSize"></param>
        /// <param name="period"></param>
        public AzureBlobLoggerProvider(string outputTemplate, string appName, string fileName, int batchSize, TimeSpan period)
        {
            _outputTemplate = outputTemplate;
            _appName = appName;
            _fileName = fileName;
            _batchSize = batchSize;
            _period = period;
        }

        /// <inheritdoc />
        public override Logger ConfigureLogger(IWebAppLogConfigurationReader reader)
        {
            var messageFormatter = new MessageTemplateTextFormatter(_outputTemplate, null);
            var container = new CloudBlobContainer(new Uri(reader.Current.BlobContainerUrl));
            var azureBlobSink = new AzureBlobSink(container, _appName, _fileName, messageFormatter, _batchSize, _period);
            var backgroundSink = new BackgroundSink(azureBlobSink, BackgroundSink.DefaultLogMessagesQueueSize);
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

            loggerConfiguration.WriteTo.Sink(backgroundSink);
            loggerConfiguration.MinimumLevel.ControlledBy(new WebConfigurationReaderLevelSwitch(reader,
                configuration =>
                {
                    return configuration.BlobLoggingEnabled ? configuration.BlobLoggingLevel: LogLevel.None;
                }));

            return loggerConfiguration.CreateLogger();
        }

    }
}