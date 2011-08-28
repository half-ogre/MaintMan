using System;

namespace MaintMan
{
    public interface IHttpRequestExecutor
    {
        void Execute(
            Uri uri,
            string userAgent,
            string method,
            string contentType,
            string content = null);
    }
}