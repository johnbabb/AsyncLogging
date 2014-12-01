namespace AsyncLogging.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using AsyncLogging.Loggers;
    using AsyncLogging.Properties;

    public class InsertServerRequestLogCommand
    {

        private delegate void InsertInfoDelegate(int rows);

        private string connectionName;

        private string sqlInsertStatment = @"
            INSERT [ServerRequestLogs]
            ([RequestDate],[RequestDateInTicks],[RequestBy],[RequestMethod],[RequestUrl],[RequestBody],[ResponseCode],[ResponseBody],[Host],[CreatedOn])
            VALUES(@RequestDate,@RequestDateInTicks,@RequestBy,@RequestMethod,@RequestUrl,@RequestBody,@ResponseCode,@ResponseBody,@Host,@CreatedOn)
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

            return result;
        }

        public IAsyncResult BeginExecuteNonQuery(ServerRequestLog logData)
        {
            SqlCommand command = null;
            try
            {
                this.connection = new SqlConnection(GetAsyncConnectionString(this.connectionName));


                command = this.GetInsertCommand(logData, this.connection);
                this.connection.Open();

                return command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                if (this.connection != null)
                {
                    this.connection.Close();
                }
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return null;
        }

        public void EndExecuteNonQuery(IAsyncResult result)
        {
            var rowCount = 0;
            try
            {
                var command = result as Task<int>;
                rowCount = command.Result;

            }
            catch (Exception ex)
            {
                rowCount = -1;
            }
            
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
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
            command.Parameters.Add("@RequestDateInTicks", SqlDbType.BigInt).Value = obj.RequestDateInTicks;
            command.Parameters.Add("@RequestBy", SqlDbType.VarChar).Value = obj.RequestBy;
            command.Parameters.Add("@RequestMethod", SqlDbType.VarChar).Value = obj.RequestMethod;
            command.Parameters.Add("@RequestUrl", SqlDbType.NVarChar).Value = obj.RequestUrl;
            command.Parameters.Add("@RequestBody", SqlDbType.NVarChar).Value = obj.RequestBody;
            command.Parameters.Add("@ResponseCode", SqlDbType.Int).Value = obj.ResponseCode;
            command.Parameters.Add("@ResponseBody", SqlDbType.NVarChar).Value = obj.ResponseBody;
            command.Parameters.Add("@Host", SqlDbType.VarChar).Value = obj.Host;
            command.Parameters.Add("@CreatedOn", SqlDbType.Date).Value = DateTime.Now;

            return command;
        }

        private SqlConnection GetSqlConnection(string connectionName)
        {
            var connString = "";
            
            connString = GetConnectionString(connectionName);
            try
            {
                return new SqlConnection(connString);
            } catch
            {
                return null;
            }
        }

        private static string GetConnectionString(string connectionName)
        {
            var connString = "";
            
            if (ConfigurationManager.ConnectionStrings.Count > 0
                && ConfigurationManager.ConnectionStrings[connectionName] != null)
            {
                connString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
            else
            {
                connString = connectionName;
            }

            return connString;
        }

        private static string GetAsyncConnectionString(string connectionName)
        {
            var connString = GetConnectionString(connectionName);
            if (string.IsNullOrEmpty(connString))
            {
                return connString;
            }

            if (!connString.ToLower().Contains("asynchronous"))
            {
                connString = connString.Trim(';', ' ') + "; Asynchronous Processing=true";
            }

            return connString;
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