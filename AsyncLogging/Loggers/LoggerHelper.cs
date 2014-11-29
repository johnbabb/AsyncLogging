
namespace AsyncLogging.Loggers
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;

    using AsyncLogging.Filters;

    public static class LoggerHelper
    {
        public static string GetOutputFilterStreamContents(OutputFilterStream filter)
        {
            if (filter != null)
            {
                return filter.ReadStream();
            }
            return null;
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

        public static bool IsLoggingContentType(HttpRequest request, string contentTypes)
        {
            if (Regex.IsMatch(request.ContentType, contentTypes, RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsLoggingStatusCode(HttpResponse response, string statusCodes)
        {
            if (Regex.IsMatch(response.StatusCode.ToString(), statusCodes))
            {
                return true;
            }
            return false;
        }

        public static ServerRequestLog InitializeServerRequestLog(HttpApplication app, OutputFilterStream filter)
        {
            DateTime time = DateTime.Now;
            var strRequest = LoggerHelper.GetDocumentContents(app.Request);
            var strResponse = LoggerHelper.GetOutputFilterStreamContents(filter);
            var nameOrAddress = LoggerHelper.GetNameOrAddress(app);

            return new ServerRequestLog()
                       {
                           Host = app.Request.Url.Host,
                           RequestBody = strRequest,
                           RequestBy = nameOrAddress,
                           RequestDate = time,
                           RequestDateInTicks = time.Ticks,
                           RequestMethod = app.Request.HttpMethod,
                           RequestUrl = app.Request.RawUrl,
                           ResponseBody = strResponse,
                           ResponseCode = app.Response.StatusCode
                       };
        }
    }
}
