
namespace AsyncLogging
{
    using System.Web;
    using AsyncLogging.Loggers;
    

    public class AsyncRequestLogModule : IHttpModule
    {
        private ILogger Logger;

        public void Init(HttpApplication application)
        {
            AsyncConfig.InitializeSettings();
            
            if (AsyncConfig.Enabled)
            {
                this.Logger = LogFactory.Make(AsyncConfig.LoggerType);
                application.BeginRequest += this.Logger.InitializeRequestHandler;
                application.AddOnEndRequestAsync(
                    this.Logger.BeginRequestAsyncEventHandler,
                    this.Logger.EndRequestAsyncEventHandler);
            }
        }

        public void Dispose()
        {
            if (Logger != null)
            {
                Logger.Dispose();
            }
        }
    }
}
