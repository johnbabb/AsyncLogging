
namespace AsyncLogging
{
    using System;
    using System.Web;

    using AsyncLogging.Filters;
    using AsyncLogging.Loggers;
    

    public class AsyncRequestLogModule : IHttpModule
    {
        private ILogger Logger;
        
        private static string FilterName = "AsyncLogHandlerFilter";

        public void Init(HttpApplication application)
        {
            AsyncConfig.InitializeSettings();

            if (AsyncConfig.Enabled && 
                LoggerHelper.IsLoggingContentType(application.Request, AsyncConfig.ContentTypes) &&
                LoggerHelper.IsLoggingStatusCode(application.Response, AsyncConfig.StatusCodes))
            {
                this.Logger = LogFactory.Make(AsyncConfig.LoggerType);

                application.BeginRequest += this.InitializeRequestHandler;
                application.BeginRequest += this.Logger.InitializeRequestHandler;

                application.AddOnEndRequestAsync(
                    this.BeginRequestAsyncEventHandler,
                    this.EndRequestAsyncEventHandler);
            }
        }

        public void Dispose()
        {
            if (Logger != null)
            {
                Logger.Dispose();
            }
        }

        public void InitializeRequestHandler(object source, EventArgs e)
        {
            var app = source as HttpApplication;
            var filter = new OutputFilterStream(app.Response.Filter);
            app.Response.Filter = filter;
            HttpContext.Current.Items.Add(FilterName, filter);
        }

        private IAsyncResult BeginRequestAsyncEventHandler(Object source, EventArgs e, AsyncCallback cb, Object state)
        {
            var filter = HttpContext.Current.Items[FilterName] as OutputFilterStream;
            return this.Logger.BeginRequestAsyncEventHandler(source, e, cb, state, filter);
        }

        private void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            this.Logger.EndRequestAsyncEventHandler(ar);
        }
    }
}
