namespace AsyncLogging.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    using AsyncLogging.Loggers;
    using AsyncLogging.Properties;

    public class InsertServerRequestLogCommand
    {
        

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
                return Settings.Default.ConnectionName;
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
                return Settings.Default.SqlInsertStatement.Trim();
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

        public InsertServerRequestLogCommand() : this(Settings.Default.ConnectionName)
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

        public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, Object stateObject)
        {
            SqlCommand command = null;
            try
            {               
                //connection = new SqlConnection(GetConnectionString());
                // To emulate a long-running query, wait for  
                // a few seconds before working with the data. 
                // This command does not do much, but that's the point-- 
                // it does not change your data, in the long run. 
                string commandText =
                    "WAITFOR DELAY '0:0:05';" +
                    "UPDATE Production.Product SET ReorderPoint = ReorderPoint + 1 " +
                    "WHERE ReorderPoint Is Not Null;" +
                    "UPDATE Production.Product SET ReorderPoint = ReorderPoint - 1 " +
                    "WHERE ReorderPoint Is Not Null";

                command = new SqlCommand(commandText, connection);
                connection.Open();

               
                // Although it is not required that you pass the  
                // SqlCommand object as the second parameter in the  
                // BeginExecuteNonQuery call, doing so makes it easier 
                // to call EndExecuteNonQuery in the callback procedure.

                return command.BeginExecuteNonQuery(callback, command);

            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return null;
        }

        public void EndExecuteNonQuery(IAsyncResult result)
        {
            try
            {
                var command = (SqlCommand)result.AsyncState;
                int rowCount = command.EndExecuteNonQuery(result);

            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

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
            
            if(ConfigurationManager.ConnectionStrings.Count > 0 && ConfigurationManager.ConnectionStrings[connectionName] != null)
            {
                connString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
            else
            {
                connString = connectionName;
            }
            try
            {
                return new SqlConnection(connString);
            } catch
            {
                return null;
            }
        }
    }
}