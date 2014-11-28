namespace AsyncLogging.Tests.SqlCommands
{
    using AsyncLogging.SqlCommands;

    public class InsertServerRequestLogCommandFixture : InsertServerRequestLogCommand
    {
        public InsertServerRequestLogCommandFixture()
            : base()
        {
            
        }
        public InsertServerRequestLogCommandFixture(string connecction, string sql) : base(connecction)
        {
            this.SqlInsertStatment = sql;
        }

        public string GetBaseInsertCommandSql()
        {
            return this.GetInsertCommandSql();
        }
    }
}