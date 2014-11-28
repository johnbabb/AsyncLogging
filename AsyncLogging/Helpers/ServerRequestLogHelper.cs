using System;

namespace AsyncLogging.Helpers
{
    using System.IO;
    using System.Web;

    using AsyncLogging.Filters;
    using AsyncLogging.Loggers;

    public class ServerRequestLogHelper
    {
        public static ServerRequestLog Initialize(Object source)
        {
            var app = (HttpApplication)source;
            var log = new ServerRequestLog();


            return log;
        }
        
        private static object GetNameOrAddress(HttpApplication app)
        {
            if (app.User != null && app.User.Identity.IsAuthenticated)
            {
                return app.User.Identity.Name;
            }

            return app.Request.UserHostAddress;
        }

        private string GetResponseContents()
        {
            var stream = HttpContext.Current.Items["AsyncLogHandlerFilter"] as OutputFilterStream;
            return stream.ReadStream();
        }

        private string GetDocumentContents(HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Request.ContentEncoding))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }
    }
}
