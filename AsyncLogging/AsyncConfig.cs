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
                var strEnabled = _settings.FirstOrDefault(w => w.Key == "Enabled").Value;
                bool isEnabled;
                bool.TryParse(strEnabled, out isEnabled);
                return isEnabled;
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

        public static string CreateTableScript
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "CreateTableScript").Value;
            }
        }

        public static string DefaultInsertSql
        {
            get
            {
                return _settings.FirstOrDefault(w => w.Key == "DefaultInsertSql").Value;
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
            Settings.GetOrAdd("CreateTableScript", @"CREATE TABLE [dbo].[ServerRequestLogs](
	                                                [ServerRequestLogId] [int] IDENTITY(1,1) NOT NULL,
	                                                [RequestDate] [datetime] NOT NULL,
	                                                [RequestBy] [varchar](50) NOT NULL,
	                                                [RequestMethod] [varchar](10) NOT NULL,
	                                                [RequestUrl] [nvarchar](2000) NOT NULL,
	                                                [RequestBody] [nvarchar](4000) NULL,
	                                                [ResponseCode] [int] NOT NULL,
	                                                [ResponseBody] [nvarchar](4000) NOT NULL,
	                                                [Host] [varchar](250) NOT NULL,
                                                 CONSTRAINT [PK_IISServerLog] PRIMARY KEY CLUSTERED 
                                                (
	                                                [ServerRequestLogId] ASC
                                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                ) ON [PRIMARY]");
            
            Settings.GetOrAdd("DefaultInsertSql",@"INSERT [ServerRequestLogs]
                                              ([RequestDate],[RequestBy],[RequestMethod],[RequestUrl],[RequestBody],[ResponseCode],[ResponseBody],[Host])
                                              VALUES(@RequestDate,@RequestBy,@RequestMethod,@RequestUrl,@RequestBody,@ResponseCode,@ResponseBody,@Host)");

        }
    }
}
