using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogging
{
    using System.Collections.Concurrent;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Policy;

    using AsyncLogging.Enums;

    public static class AsyncConfig
    {
        private static readonly ConcurrentDictionary<string, string> _settings = new ConcurrentDictionary<string, string>();

        public static ConcurrentDictionary<string, string> Settings
        {
            get
            {
                return _settings;
            }
        }

        public static LoggerType LoggerType
        {
            get
            {
                if (!_settings.Any(w => w.Key == "LoggerType"))
                {
                    return LoggerType.File;
                }

                var logType = _settings.FirstOrDefault(w => w.Key == "LoggerType").Value;
                
                LoggerType myEnum;
                if (!Enum.TryParse(logType, true, out myEnum))
                {
                    return LoggerType.File;
                }

                return myEnum;                  
            }
        }

        public static string ConnectionName
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "ConnectionName").Value;
            }
        }

        public static string LogfileFullName
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "LogfileFullName").Value;
            }
        }

        public static bool Enabled
        {
            get
            {
                if (!_settings.Any(w => w.Key == "Enabled"))
                {
                    return false;
                }

                if (_settings.FirstOrDefault(w => w.Key == "Enabled").Value.ToLower() == "false")
                {
                    return false;
                }

                return true;
            }
        }

        public static string StatusCodes
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "StatusCodes").Value;
            }
        }

        public static string SqlInsertStatement
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "SqlInsertStatement").Value;
            }
        }

        public static string ContentTypes
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "ContentTypes").Value;
            }
        }

        public static void InitializeSettings()
        {
            Settings.GetOrAdd("LoggerType", Properties.Settings.Default.LoggerType);
            Settings.GetOrAdd("ConnectionName", Properties.Settings.Default.ConnectionName);
            Settings.GetOrAdd("LogfileFullName", Properties.Settings.Default.LogfileFullName);
            Settings.GetOrAdd("Enabled", Properties.Settings.Default.Enabled);
            Settings.GetOrAdd("StatusCodes", Properties.Settings.Default.StatusCodes);
            Settings.GetOrAdd("SqlInsertStatement", Properties.Settings.Default.SqlInsertStatement);
            Settings.GetOrAdd("ContentTypes", Properties.Settings.Default.ContentTypes);
        }
    }
}
