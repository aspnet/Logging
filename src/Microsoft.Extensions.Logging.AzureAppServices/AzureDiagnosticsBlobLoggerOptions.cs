// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.AzureAppServices
{
    public class AzureDiagnosticsBlobLoggerOptions: BatchingLoggerOptions
    {
        private string _blobName = "applicationLog.txt";

        /// <summary>
        /// Gets or sets the last section of log blob name.
        /// Defaults to <c>"applicationLog.txt"</c>.
        /// </summary>
        public string BlobName
        {
            get { return _blobName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(nameof(value), $"{nameof(BlobName)} must be non-empty string.");
                }
                _blobName = value;
            }
        }

        /// <summary>
        /// Gets of sets the SAS endpoint where blob logs are stored.
        /// </summary>
        public string ContainerUrl { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationInstanceId { get; set; }
    }
}