using System;
using System.IO;
using System.Net;

namespace MaintMan
{
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        public void Execute(
            Uri uri, 
            string userAgent, 
            string method, 
            string contentType, 
            string content = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.UserAgent = userAgent;
            request.ContentType = contentType;
            request.Method = method;

            if (!string.IsNullOrWhiteSpace(content))
            {
                request.ContentLength = content.Length;
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    writer.Write(content);
            }

            using (var httpResponse = request.GetResponse())
            {
            }
        }
    }
}