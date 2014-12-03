namespace AsyncLogging.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Remoting.Messaging;
    using System.Threading.Tasks;

    using AsyncLogging.Extensions;
    using AsyncLogging.Loggers;
    using AsyncLogging.Properties;

    public class InsertServerRequestLogCommand
    {

        private delegate void InsertInfoDelegate(int rows);

        private string connectionName;

        private string sqlInsertStatment = @"
            INSERT [ServerRequestLogs]
            ([RequestDate],[RequestBy],[RequestMethod],[RequestUrl],[RequestBody],[ResponseCode],[ResponseBody],[Host])
            VALUES(@RequestDate,@RequestBy,@RequestMethod,@RequestUrl,@RequestBody,@ResponseCode,@ResponseBody,@Host)
            ";

        protected SqlConnection connection;

        public string DefaultConnectionName
        {
            get
            {
                return AsyncConfig.ConnectionName;
            }
        }

        public string ConnectionName
        {
            get
            {
                return connectionName;
            }
        }

        public string DefaultSqlInsertStatement
        {
            get
            {
                return AsyncConfig.SqlInsertStatement.Trim();
            }
        }

        protected string SqlInsertStatment {
            get
            {
                return this.sqlInsertStatment;
            }
            set
            {
                this.sqlInsertStatment = value;
            }
        }

        public InsertServerRequestLogCommand(string connectionName)
        {
            this.connectionName = connectionName;
        }

        public InsertServerRequestLogCommand() : this(AsyncConfig.ConnectionName)
        {
        
        }

        public int Execute(ServerRequestLog obj)
        {
            int result = 0;

            try
            {
                using (var connection = this.GetSqlConnection(this.connectionName))
                {
                    if (connection == null)
                    {
                        return result;
                    }
                    var command = this.GetInsertCommand(obj, connection);
                    command.Connection.Open();
                    result = command.ExecuteNonQuery();
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            catch
            {                
                throw;
            }
            
            return result;
        }

        public IAsyncResult BeginExecuteNonQuery(AsyncCallback cb, object state, ServerRequestLog logData)
        {
            SqlCommand command = null;
            try
            {
                this.connection = new SqlConnection(GetConnectionString(this.connectionName));


                command = this.GetInsertCommand(logData, this.connection);
                this.connection.Open();

                return command.ExecuteNonQueryAsync().ToApm(cb, state);

            }
            catch
            {
                if (this.connection != null)
                {
                    this.connection.Close();
                }
                if (command != null)
                {
                    command.Dispose();
                }
                throw;
            }
        }

        public void EndExecuteNonQuery(IAsyncResult result)
        {
            var rowCount = 0;
            try
            {
                var command = result as Task<int>;
                rowCount = command.Result;

            }
            catch
            {
                rowCount = -1;
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                throw;
            }
            this.SetRowRount(rowCount);
        }

        protected string GetInsertCommandSql()
        {
            var configSqlCommand = this.DefaultSqlInsertStatement;
            if (!string.IsNullOrEmpty(configSqlCommand))
            {
                return configSqlCommand;
            }
            return this.SqlInsertStatment;
        }

        private SqlCommand GetInsertCommand(ServerRequestLog obj, SqlConnection connection)
        {
            var command = new SqlCommand(this.GetInsertCommandSql(), connection);

            command.Parameters.Add("@RequestDate", SqlDbType.DateTime).Value = obj.RequestDate;
            command.Parameters.Add("@RequestBy", SqlDbType.VarChar).Value = obj.RequestBy;
            command.Parameters.Add("@RequestMethod", SqlDbType.VarChar).Value = obj.RequestMethod;
            command.Parameters.Add("@RequestUrl", SqlDbType.NVarChar).Value = obj.RequestUrl;
            command.Parameters.Add("@RequestBody", SqlDbType.NVarChar).Value = obj.RequestBody;
            command.Parameters.Add("@ResponseCode", SqlDbType.Int).Value = obj.ResponseCode;
            command.Parameters.Add("@ResponseBody", SqlDbType.NVarChar).Value = obj.ResponseBody;
            command.Parameters.Add("@Host", SqlDbType.VarChar).Value = obj.Host;

            return command;
        }

        private SqlConnection GetSqlConnection(string connectionName)
        {
            var connString = "";
            
            connString = GetConnectionString(connectionName);
            try
            {
                return new SqlConnection(connString);
            }
            catch
            {
                throw;
            }
        }

        private static string GetConnectionString(string connectionName)
        {
            
            if (ConfigurationManager.ConnectionStrings.Count > 0
                && ConfigurationManager.ConnectionStrings[connectionName] != null)
            {
                return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }

            throw new ArgumentException("Connection with the connectionName: " + connectionName + " is not found in the ConfigurationManager.ConnectionStrings sections.  Please add the connection string to this section, or use another named connection from this section.");
        }

        private int _rowCount = 0;
        private void SetRowRount(int rowCount)
        {
            _rowCount = rowCount;
        }

        public int GetRowCount()
        {
            return _rowCount;
        }
    }
}