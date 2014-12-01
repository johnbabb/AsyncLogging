namespace AsyncLogging.Loggers.Imp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;

    using AsyncLogging.Filters;
    using AsyncLogging.Helpers;

    public class FileLogger : ILogger
    {
        private static long position = 0;
        private static object @lock = new object();
        private FileStream file; 
           

        public void InitializeRequestHandler(object source, EventArgs e)
        {

        }

        public IAsyncResult BeginRequestAsyncEventHandler(object source, EventArgs e, AsyncCallback cb, object state, OutputFilterStream filter)
        {
            var app = source as HttpApplication;
            var logData = LoggerHelper.InitializeServerRequestLog(app, filter);
            string line = FormatRow(logData);
            
            return this.WriteLog(line, cb, state);
        }
        
        public void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            this.file.EndWrite(ar);
            this.file.Close();
        }

        public void Dispose()
        {
           
        }

        protected string FormatRow(ServerRequestLog logData)
        {
            string line = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}\r\n",
                logData.RequestDateInTicks, logData.ResponseCode, logData.RequestBy, logData.RequestMethod, logData.RequestUrl, logData.RequestBody, logData.ResponseBody);
            
            return line;
        }

        protected IAsyncResult WriteLog(string line, AsyncCallback cb, object state)
        {
            byte[] output = Encoding.ASCII.GetBytes(line);
            lock (@lock)
            {
                this.file = new FileStream(this.LogFileFullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024, true);
                this.file.Seek(position, SeekOrigin.Begin);
                position += output.Length;
                return this.file.BeginWrite(output, 0, output.Length, cb, state);
            }
        }
        
        protected string LogFileFullName
        {
            get
            {
                return HttpContext.Current.Server.MapPath(AsyncConfig.LogfileFullName);
            }
        }
    }
}