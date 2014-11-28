namespace AsyncLogging.Loggers.Imp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;

    using AsyncLogging.Filters;

    public class FileLogger : ILogger
    {
        private static long position = 0;
        private static object @lock = new object();
        private FileStream file;               

        public void InitializeRequestHandler(object source, EventArgs e)
        {
            var settings = AsyncConfig.Settings;
            HttpApplication app = (HttpApplication)source;
            var filter = new OutputFilterStream(app.Response.Filter);
            app.Response.Filter = filter; 
            HttpContext.Current.Items.Add("AsyncLogHandlerFilter", filter);
        }

        public IAsyncResult BeginRequestAsyncEventHandler(object source, EventArgs e, AsyncCallback cb, object state)
        {
            HttpApplication app = (HttpApplication)source;
            DateTime time = DateTime.Now;
            var strRequest = LoggerHelper.GetDocumentContents(app.Request);
            var strResponse = LoggerHelper.GetResponseContents();
            var nameOrAddress = LoggerHelper.GetNameOrAddress(app);
            string line = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}\r\n", time.Ticks, app.Response.StatusCode, nameOrAddress, app.Request.HttpMethod, app.Request.Url, strRequest, strResponse);
            byte[] output = Encoding.ASCII.GetBytes(line);
            lock (@lock)
            {
                this.file = new FileStream(HttpContext.Current.Server.MapPath("~/App_Data/RequestLog.txt"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024, true);
                this.file.Seek(position, SeekOrigin.Begin);
                position += output.Length;
                return this.file.BeginWrite(output, 0, output.Length, cb, state);
            }
        }

        public void EndRequestAsyncEventHandler(IAsyncResult ar)
        {
            this.file.EndWrite(ar);
            this.file.Close();
        }

        public void Dispose()
        {
           
        }
    }
}