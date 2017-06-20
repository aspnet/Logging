// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    /// <inheritdoc />
    public class BlobAppendReferenceWrapper : ICloudAppendBlob
    {
        private readonly string _containerUrl;
        private Uri _fullUri;
        private HttpClient _client;
        private Uri _appendUri;

        public BlobAppendReferenceWrapper(string containerUrl, string name) : this(containerUrl, name, new HttpClientHandler())
        {
        }

        public BlobAppendReferenceWrapper(string containerUrl, string name, HttpMessageHandler handler)
        {
            _containerUrl = containerUrl;
            var uri = new Uri(_containerUrl);
            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Path += "/" + name;
            _fullUri = uriBuilder.Uri;
            uriBuilder.Query += "&comp=appendblock";
            _appendUri = uriBuilder.Uri;
            _client = new HttpClient(handler);
        }

        /// <inheritdoc />
        public async Task AppendAsync(Stream stream, CancellationToken cancellationToken)
        {
            async Task<HttpResponseMessage> AppendDataAsync()
            {
                var message = new HttpRequestMessage(HttpMethod.Put, _appendUri)
                {
                    Headers =
                    {
                        {"x-ms-blob-type", "AppendBlob"},
                        {"x-ms-version", "2015-12-11"}
                    },
                    Content = new StreamContent(stream),
                };
                message.Headers.Date = DateTimeOffset.UtcNow;

                return await _client.SendAsync(message, cancellationToken);
            }

            var response = await AppendDataAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // If no blob exists try creating it
                var message = new HttpRequestMessage(HttpMethod.Put, _fullUri)
                {
                    Headers =
                    {
                        // Include conditional header to avoid race during creation
                        {"If-None-Match", "*"},
                        {"x-ms-blob-type", "AppendBlob"}
                    },
                    // Set Content-Length to 0 to create "Append Blob"
                    Content = new ByteArrayContent(Array.Empty<byte>())

                };
                var createResponse =
                    await _client.SendAsync(message, cancellationToken);

                // If result and not 200 or 412 throw, we don't know what to do with it
                if (!createResponse.IsSuccessStatusCode &&
                    createResponse.StatusCode != HttpStatusCode.PreconditionFailed)
                {
                    createResponse.EnsureSuccessStatusCode();
                }

                // Retry sending data after blob creation
                response = await AppendDataAsync();
            }

            response.EnsureSuccessStatusCode();
        }
    }
}