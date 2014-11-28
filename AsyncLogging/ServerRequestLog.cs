namespace AsyncLogging.Loggers
{
    using System;

    public class ServerRequestLog
    {
        public int ServerRequestLogId { get; set; }

        public DateTime RequestDate { get; set; }

        public long RequestDateInTicks { get; set; }

        public string RequestBy { get; set; }

        public string RequestMethod { get; set; }

        public string RequestUrl { get; set; }

        public string RequestBody { get; set; }

        public int ResponseCode { get; set; }

        public string ResponseBody { get; set; }

        public string Host { get; set; }
    }
}