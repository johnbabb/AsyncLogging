
namespace AsyncLogging.Loggers
{
    using System.IO;
    using System.Web;

    using AsyncLogging.Filters;

    public static class LoggerHelper
    {
        public static string GetResponseContents()
        {
            var stream = HttpContext.Current.Items["AsyncLogHandlerFilter"] as OutputFilterStream;
            return stream.ReadStream();
        }

        public static string GetNameOrAddress(HttpApplication app)
        {
            if (app.User != null && app.User.Identity.IsAuthenticated)
            {
                return app.User.Identity.Name;
            }

            return app.Request.UserHostAddress;
        }


        public static string GetDocumentContents(HttpRequest Request)
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
