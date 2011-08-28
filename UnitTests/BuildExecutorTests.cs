using System;
using Moq;
using Xunit;
using System.Net;
using System.Text;
using System.Web;

namespace MaintMan
{
    public class BuildExecutorTests
    {
        public class The_Execute_method
        {
            [Fact]
            public void will_throw_when_the_message_is_longer_than_2048_characters()
            {
                var buildExecutor = CreateExecutor();
                var buildUri = new Uri("http://aFakeHost/aPath");

                var ex = Assert.Throws<ArgumentException>(() => buildExecutor.Execute(buildUri, "aMessageThatIsTooLong".PadRight(2049)));

                Assert.Equal("message", ex.ParamName);
                Assert.True(ex.Message.StartsWith("The message may not be longer than 2048 characters."));
            }

            [Fact]
            public void will_send_an_http_request_to_the_build_url()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");

                buildExecutor.Execute(buildUri);

                httpRequestExecutor.Verify(x => x.Execute(
                    buildUri,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()));
            }

            [Fact]
            public void will_send_an_http_request_with_the_user_agent()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");

                buildExecutor.Execute(buildUri);

                httpRequestExecutor.Verify(x => x.Execute(
                    It.IsAny<Uri>(),
                    It.Is<string>(userAgent => userAgent.StartsWith("MaintMan")),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()));
            }

            [Fact]
            public void will_send_an_http_request_with_the_POST_method()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");

                buildExecutor.Execute(buildUri);

                httpRequestExecutor.Verify(x => x.Execute(
                    It.IsAny<Uri>(),
                    It.IsAny<string>(),
                    "POST",
                    It.IsAny<string>(),
                    It.IsAny<string>()));
            }

            [Fact]
            public void will_send_an_http_request_with_the_applicaton_JSON_content_type()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");

                buildExecutor.Execute(buildUri);

                httpRequestExecutor.Verify(x => x.Execute(
                    It.IsAny<Uri>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    "application/json",
                    It.IsAny<string>()));
            }

            [Fact]
            public void will_send_an_http_request_with_the_JSON_payload()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");

                buildExecutor.Execute(buildUri);

                httpRequestExecutor.Verify(x => x.Execute(
                    It.IsAny<Uri>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    string.Format(BuildExecutor.JsonTemplate, string.Format(BuildExecutor.PayloadUrlTemplate, "http://aFakeBaseUrl"))));
            }

            [Fact]
            public void will_append_the_message_to_the_payload_url_in_the_JSON()
            {
                var httpRequestExecutor = new Mock<IHttpRequestExecutor>();
                var buildExecutor = CreateExecutor(httpRequestExecutor: httpRequestExecutor);
                var buildUri = new Uri("http://aFakeHost/aPath");
                var messageBytes = Encoding.UTF8.GetBytes("theMessage");
                var urlEncodedMessage = HttpServerUtility.UrlTokenEncode(messageBytes);

                buildExecutor.Execute(buildUri, "theMessage");

                httpRequestExecutor.Verify(x => x.Execute(
                    It.IsAny<Uri>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    string.Format(BuildExecutor.JsonTemplate, string.Format(BuildExecutor.PayloadUrlTemplate, "http://aFakeBaseUrl") + "?message=" + urlEncodedMessage)));
            }

            // TODO: skipped test for bad URL because it is a pain to mock ex.Response
        }

        public static BuildExecutor CreateExecutor(
            Mock<IConfiguration> configuration = null,
            Mock<IHttpRequestExecutor> httpRequestExecutor = null)
        {
            if (configuration == null)
            {
                configuration = new Mock<IConfiguration>();
                configuration.Setup(x => x.BaseUrl)
                    .Returns("http://aFakeBaseUrl");
            }
            
            httpRequestExecutor = httpRequestExecutor ?? new Mock<IHttpRequestExecutor>();

            return new BuildExecutor(
                configuration.Object, 
                httpRequestExecutor.Object);
        }
    }
}
