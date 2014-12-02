
namespace AsyncLogging
{
    using System;
    using System.Threading.Tasks;
    using System.Web;

    using AsyncLogging.Extensions;
    using AsyncLogging.Filters;
    using AsyncLogging.Helpers;
    using AsyncLogging.Loggers;
    

    public class AsyncRequestLogModule : IHttpModule
    {
        private ILogger Logger;
        
        private static string FilterName = "AsyncLogHandlerFilter";

        private bool isLoggingApplication = false;

        public void Init(HttpApplication application)
        {
            AsyncConfig.InitializeSettings();

            if (AsyncConfig.Enabled)
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
            var application = source as HttpApplication;
            this.isLoggingApplication = LoggerHelper.IsLoggingApplication(application);
            if (this.isLoggingApplication)
            {
                var filter = HttpContext.Current.Items[FilterName] as OutputFilterStream;
                return this.Logger.BeginRequestAsyncEventHandler(source, e, cb, state, filter);    
            }
            return Task<int>.Factory.StartNew(() => -1).ToApm(cb, state);    
        }

        private void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            if (this.isLoggingApplication)
            {
                this.Logger.EndRequestAsyncEventHandler(ar);
            }
        }
    }
}
