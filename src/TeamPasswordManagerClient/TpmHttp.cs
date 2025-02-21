using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    internal class TpmHttp
    {
        private readonly string publicKey;
        private readonly string privateKey;
        private readonly string baseUrl;

        public TpmHttp(TpmConfig config)
        {
            this.publicKey = config.PublicKey;
            this.privateKey = config.PrivateKey;
            this.baseUrl = config.BaseUrl;

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            if (!baseUrl.Contains("/api/"))
            {
                baseUrl += "api/v4/";
            }
        }

        public async Task<string> Get(string url)
        {
            var request = BuildRequest("GET", url);
            return await ReadResponse(request);
        }

        public async Task<string> Delete(string url)
        {
            var request = BuildRequest("DELETE", url);
            return await ReadResponse(request);
        }

        public async Task<string> Post(string url, string body)
        {
            var request = BuildRequest("POST", url, body);
            return await ReadResponse(request);
        }

        public async Task<string> Put(string url)
        {
            var request = BuildRequest("PUT", url);
            return await ReadResponse(request);
        }

        public async Task<string> Put(string url, string body)
        {
            var request = BuildRequest("PUT", url, body);
            return await ReadResponse(request);
        }


        public WebRequest BuildRequest(string method, string url, string body = null)
        {
            var fullUrl = $"{baseUrl}{url}";
            var urlToHash = fullUrl.Substring(fullUrl.IndexOf("api/", StringComparison.InvariantCultureIgnoreCase));
            var timestamp = CurrentTimeStamp();
            var request = WebRequest.Create(fullUrl);
            request.Method = method;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("X-Public-Key", publicKey);
            request.Headers.Add("X-Request-Hash", Hash(timestamp, urlToHash, body));
            request.Headers.Add("X-Request-Timestamp", timestamp.ToString());

            if (!string.IsNullOrWhiteSpace(body))
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            return request;
        }

        public async Task<string> ReadResponse(WebRequest request)
        {
            var httpResponse = await request.GetResponseAsync();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        public async Task<IEnumerable<T>> FetchAllPages<T>(Func<int, Task<IEnumerable<T>>> getPage, int pageSize = 20)
        {
            List<T> all = new List<T>();
            List<T> current;
            int page = 1;

            do
            {
                current = (await getPage(page)).ToList();

                all.AddRange(current);

                if (current.Count > pageSize) pageSize = current.Count;
                if (current.Count < pageSize) break;

                page++;
            }
            while (current.Count == pageSize);

            return all;
        }

        private string Hash(long timestamp, string url, string body)
        {
            var keyBytes = Encoding.UTF8.GetBytes(privateKey);
            var data = url + timestamp + (body ?? "");
            return ToHex(Hmac(keyBytes, Encoding.UTF8.GetBytes(data)));
        }

        private long CurrentTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }

        private byte[] Hmac(byte[] keyBytes, byte[] data)
        {
            using (var hmac = new HMACSHA256(keyBytes))
            {
                return hmac.ComputeHash(data);
            }
        }

        private string ToHex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", String.Empty).ToLower();
        }
    }
}
