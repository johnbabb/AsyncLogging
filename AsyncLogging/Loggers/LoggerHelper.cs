
namespace AsyncLogging.Loggers
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;

    using AsyncLogging.Filters;

    public static class LoggerHelper
    {
        private static string FilterName = "AsyncLogHandlerFilter";

        public static void InitOutputFilterStream(HttpApplication app)
        {
            var filter = new OutputFilterStream(app.Response.Filter);
            app.Response.Filter = filter;
            HttpContext.Current.Items.Add(FilterName, filter);
        }

        public static string GetOutputFilterStreamContents()
        {
            var stream = HttpContext.Current.Items[FilterName] as OutputFilterStream;
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

        public static bool IsLoggingContentTypes(HttpRequest request, string contentTypes)
        {
            if (Regex.IsMatch(request.ContentType, contentTypes, RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsLoggingStatusCodes(HttpResponse response, string statusCodes)
        {
            if (Regex.IsMatch(response.StatusCode.ToString(), statusCodes))
            {
                return true;
            }
            return false;
        }
    }
}
