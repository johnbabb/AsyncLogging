namespace AsyncLogging
{
    using System;
    using System.Reflection;

    using AsyncLogging.Enums;
    using AsyncLogging.Loggers;

    public class LogFactory
    {
        public static ILogger Make(string type)
        {
            var l = (LoggerType)Enum.Parse(typeof(LoggerType), type);
            return GetInstance(l) as ILogger;
        }

        public static ILogger Make(LoggerType type)
        {
            return GetInstance(type) as ILogger;
        }
        
        public static object GetInstance(LoggerType type)
        {
            var nms ="AsyncLogging.Loggers.Imp." + type + "Logger";

            return GetInstance(nms);
        }

        public static object GetInstance(string nmspace)
        {
            Type t = Type.GetType(nmspace);
            return Activator.CreateInstance(t);
        }
    }
}