
namespace AsyncLogging.Loggers.Imp
{
    using System;

    using AsyncLogging.Filters;
    using AsyncLogging.Properties;
    using AsyncLogging.SqlCommands;

    public class SqlServerLogger : ILogger
    {
        public void Log(ServerRequestLog record)
        {
            var cmds = new InsertServerRequestLogCommand(Settings.Default.ConnectionName);
            cmds.Execute(record);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InitializeRequestHandler(object source, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequestAsyncEventHandler(object source, EventArgs e, AsyncCallback cb, object state, OutputFilterStream filter)
        {
            throw new NotImplementedException();
        }

        public void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }
    }
}
