using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

namespace MaintMan
{
    public class BuildExecutor : IBuildExecutor
    {
        // keeping this simple for now; might consider using JSON.NET
        public const string JsonTemplate =
@"{{
   ""branches"": {{
      ""default"": {{
         ""commit_id"": ""12345678"",
         ""commit_message"": ""Turning on maintenance mode."",
         ""download_url"": ""{0}""
      }}
   }}
}}";
        public const string PayloadUrlTemplate = "{0}/payload.tar.gz";

        readonly IConfiguration configuration;
        readonly IHttpRequestExecutor httpRequestExecutor;

        public BuildExecutor(
            IConfiguration configuration,
            IHttpRequestExecutor httpRequestExecutor)
        {
            this.configuration = configuration;
            this.httpRequestExecutor = httpRequestExecutor;
        }
        
        public void Execute(
            Uri buildUri,
            string message = null)
        {
            if (message != null && message.Length > 2048)
                throw new ArgumentException("The message may not be longer than 2048 characters.", "message");
            
            var payloadUrl = string.Format(PayloadUrlTemplate, configuration.BaseUrl);
            if (message != null)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                var urlEncodedMessage = HttpServerUtility.UrlTokenEncode(bytes);
                payloadUrl += "?message=" + urlEncodedMessage;
            }
            
            string json = string.Format(JsonTemplate, payloadUrl);

            try
            {
                httpRequestExecutor.Execute(
                    buildUri,
                    "MaintMan/0.1",
                    "POST",
                    "application/json",
                    json);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (StreamReader responseReader = new StreamReader(stream))
                {
                    throw new BadUrlException("Executing the build URL failed with: " + responseReader.ReadToEnd());
                }
            }
        }
    }
}