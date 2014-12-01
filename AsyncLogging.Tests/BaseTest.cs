namespace AsyncLogging.Tests
{
    using System.Configuration;
    using System.Data.SqlClient;

    using NUnit.Framework;

    public class BaseTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            AsyncConfig.InitializeSettings();
        }

        [SetUp]
        public void SetUp()
        {
            AsyncConfig.InitializeSettings();
        }

        protected SqlConnection GetSqlConnection(string connectionName)
        {
            var connString = "";

            if (ConfigurationManager.ConnectionStrings.Count > 0 && ConfigurationManager.ConnectionStrings[connectionName] != null)
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
            }
            catch
            {
                return null;
            }
        }
    }
}