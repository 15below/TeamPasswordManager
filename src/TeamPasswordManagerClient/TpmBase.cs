using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TeamPasswordManagerClient
{
    public abstract class TpmBase
    {
        private readonly string publicKey;
        private readonly string privateKey;
        private readonly string baseUrl;

        public TpmBase(TpmConfig config)
        {
            this.publicKey = config.PublicKey;
            this.privateKey = config.PrivateKey;
            this.baseUrl = config.BaseUrl;
        }

        protected string Get(string url)
        {
            var request = BuildRequest("GET", url);
            return ReadResponse(request);
        }

        protected string Delete(string url)
        {
            var request = BuildRequest("DELETE", url);
            return ReadResponse(request);
        }

        protected string Post(string url, string body)
        {
            var request = BuildRequest("POST", url, body);
            AddBody(body, request);
            return ReadResponse(request);
        }

        protected string Put(string url)
        {
            var request = BuildRequest("PUT", url);
            return ReadResponse(request);
        }

        protected string Put(string url, string body)
        {
            var request = BuildRequest("PUT", url, body);
            AddBody(body, request);
            return ReadResponse(request);
        }

        protected WebRequest BuildRequest(string method, string url)
        {
            var timestamp = CurrentTimeStamp();
            var request = WebRequest.Create(baseUrl + url);
            request.Method = method;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("X-Public-Key", publicKey);
            request.Headers.Add("X-Request-Hash", Hash(timestamp, url));
            request.Headers.Add("X-Request-Timestamp", timestamp.ToString());
            return request;
        }

        protected WebRequest BuildRequest(string method, string url, string body)
        {
            var timestamp = CurrentTimeStamp();
            var request = WebRequest.Create(baseUrl + url);
            request.Method = method;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("X-Public-Key", publicKey);
            request.Headers.Add("X-Request-Hash", Hash(timestamp, url, body));
            request.Headers.Add("X-Request-Timestamp", timestamp.ToString());
            return request;
        }

        protected static void AddBody(string body, WebRequest request)
        {
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        protected static string ReadResponse(WebRequest request)
        {
            var httpResponse = request.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        protected List<T> FetchAllPages<T>(Func<int, List<T>> getPage, int pageSize = 20)
        {
            List<T> all = new List<T>();
            List<T> current;
            int page = 1;

            do
            {
                current = getPage(page);

                all.AddRange(current);

                if (current.Count > pageSize) pageSize = current.Count;
                if (current.Count < pageSize) break;

                page++;
            }
            while (current.Count == pageSize);

            return all;
        }

        private string Hash(long timestamp, string url, string body = null)
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
