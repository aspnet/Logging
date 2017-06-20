using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.AzureAppServices.Internal;
using Xunit;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test
{
    public class AzureAppendBlobTests
    {
        public string _containerUrl = "https://host/container?query=1";
        public string _blobName = "blob/path";

        [Fact]
        public async Task SendsDataAsStream()
        {
            var testMessageHandler = new TestMessageHandler(async message =>
            {
                Assert.Equal(HttpMethod.Put, message.Method);
                Assert.Equal("https://host/container/blob/path?query=1", message.RequestUri.ToString());
                Assert.Equal(new byte[] { 0, 2, 3 }, await message.Content.ReadAsByteArrayAsync());

                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var blob = new BlobAppendReferenceWrapper(_containerUrl, _blobName, testMessageHandler);
            await blob.AppendAsync(new MemoryStream(new byte[] { 0, 2, 3 }), CancellationToken.None);
        }

        [Fact]
        public async Task CreatesBlobIfNotExist()
        {
            var stage = 0;
            var testMessageHandler = new TestMessageHandler(async message =>
            {
                // First PUT request
                if (stage == 0)
                {
                    Assert.Equal(HttpMethod.Put, message.Method);
                    Assert.Equal("https://host/container/blob/path?query=1", message.RequestUri.ToString());
                    stage = 1;
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                // First PUT request
                else if (stage == 1)
                {
                    Assert.Equal(HttpMethod.Put, message.Method);
                    Assert.Equal("https://host/container/blob/path?query=1", message.RequestUri.ToString());
                    stage = 1;
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            });

            var blob = new BlobAppendReferenceWrapper(_containerUrl, _blobName, testMessageHandler);
            await blob.AppendAsync(new MemoryStream(new byte[] { 0, 2, 3 }), CancellationToken.None);
        }


        private class TestMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _callback;

            public TestMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> callback)
            {
                _callback = callback;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await _callback(request);
            }

        }
    }
}
