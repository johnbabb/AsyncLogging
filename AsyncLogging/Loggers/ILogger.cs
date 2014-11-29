namespace AsyncLogging.Loggers
{
    using System;

    using AsyncLogging.Filters;

    public interface ILogger : IDisposable
    {
        void InitializeRequestHandler(Object source, EventArgs e);

        IAsyncResult BeginRequestAsyncEventHandler(Object source, EventArgs e, AsyncCallback cb, Object state, OutputFilterStream filter);

        void EndRequestAsyncEventHandler(IAsyncResult ar);

    }
}
