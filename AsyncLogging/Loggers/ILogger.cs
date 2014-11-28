namespace AsyncLogging.Loggers
{
    using System;

    public interface ILogger : IDisposable
    {
        void InitializeRequestHandler(Object source, EventArgs e);

        IAsyncResult BeginRequestAsyncEventHandler(Object source, EventArgs e, AsyncCallback cb, Object state);

        void EndRequestAsyncEventHandler(IAsyncResult ar);

    }
}
