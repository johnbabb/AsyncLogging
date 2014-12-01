
namespace AsyncLogging.Loggers.Imp
{
    using System;
    using System.Web;

    using AsyncLogging.Filters;
    using AsyncLogging.Properties;
    using AsyncLogging.SqlCommands;

    public class SqlServerLogger : ILogger
    {
        private InsertServerRequestLogCommand command;
        
        public void Dispose()
        {
            
        }

        public void InitializeRequestHandler(object source, EventArgs e)
        {
            command = new InsertServerRequestLogCommand();
        }

        public IAsyncResult BeginRequestAsyncEventHandler(object source, EventArgs e, AsyncCallback cb, object state, OutputFilterStream filter)
        {
            var app = source as HttpApplication;
            var logData = LoggerHelper.InitializeServerRequestLog(app, filter);
            return this.command.BeginExecuteNonQuery(cb, state, logData);
        }

        public void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            this.command.EndExecuteNonQuery(ar);
        }
    }
}
