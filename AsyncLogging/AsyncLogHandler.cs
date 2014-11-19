//http://msdn.microsoft.com/en-us/magazine/cc163463.aspx#S3
//http://www.paulallen.org/blog/capture-and-log-requests-and-responses-aspnet-mvc-3
//http://stackoverflow.com/questions/2841974/how-to-read-the-response-stream-before-the-http-response-completes
namespace AsyncLogging
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.IO;
    using System.Text;

    public class AsyncRequestLogModule : IHttpModule
    {
        private static long position = 0;
        private static object @lock = new object();
        private FileStream file;
        

        public void Init(HttpApplication application)
        {
            application.BeginRequest += BeginRequestHandlerExecute;
            application.AddOnEndRequestAsync(this.BeginPostRequestHandlerExecute, this.EndPreRequestHandlerExecute);
        }

        private IAsyncResult BeginPostRequestHandlerExecute(Object source, EventArgs e, AsyncCallback cb, Object state)
        {
           

            HttpApplication app = (HttpApplication)source;
            DateTime time = DateTime.Now;
            var strRequest = GetDocumentContents(app.Request);
            var strResponse = GetResponseContents();
            var nameOrAddress = GetNameOrAddress(app);
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

        private object GetNameOrAddress(HttpApplication app)
        {
            if (app.User != null && app.User.Identity.IsAuthenticated)
            {
                return app.User.Identity.Name;
            }

            return app.Request.UserHostAddress;
        }

        public void Dispose()
        {

        }

        private void BeginRequestHandlerExecute(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            var filter = new OutputFilterStream(app.Response.Filter);
            app.Response.Filter = filter; 
            HttpContext.Current.Items.Add("AsyncLogHandlerFilter", filter);
        }

        private void EndPreRequestHandlerExecute(IAsyncResult ar)
        {
            this.file.EndWrite(ar);
            this.file.Close();
        }

        private string GetResponseContents()
        {
            var stream = HttpContext.Current.Items["AsyncLogHandlerFilter"] as OutputFilterStream;
            return stream.ReadStream();
        }

        private string GetDocumentContents(HttpRequest Request)
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
