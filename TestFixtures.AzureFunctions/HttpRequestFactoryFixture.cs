using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestFixtures.AzureFunctions
{
    public static class HttpRequestFactoryFixture
    {
        private static Dictionary<string, StringValues> CreateDictionary(string key, string value)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key, value }
            };
            return qs;
        }

        public static DefaultHttpRequest CreateHttpGetRequest(string queryStringKey, string queryStringValue)
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue))
            };
            return request;
        }

        public static DefaultHttpRequest CreateHttpPostRequest(string jsonBody = null)
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                ContentType = "application/json"
            };

            if (!string.IsNullOrWhiteSpace(jsonBody))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(jsonBody);
                var stream = new MemoryStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0; // Set position to beginning of the stream so it can be read.

                request.Body = stream;
            }

            return request;
        }
    }
}
