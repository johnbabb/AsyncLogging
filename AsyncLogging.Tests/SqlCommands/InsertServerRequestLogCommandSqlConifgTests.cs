
namespace AsyncLogging.Tests.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net.Mime;
    using System.Reflection;
    using System.Xml;

    using AsyncLogging.Loggers;
    using AsyncLogging.SqlCommands;

    using NUnit.Framework;

    [TestFixture]
    public class InsertServerRequestLogCommandSqlConifgTests
    {
        private InsertServerRequestLogCommandFixture classUnderTest;

        private DateTime CurrentDate;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.CurrentDate = DateTime.Now;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var connection = this.GetSqlConnection("RSAudit"))
            {
                connection.Open();
                var command = new SqlCommand("delete from ServerRequestLogs where RequestDateInTicks = " + this.CurrentDate.Ticks, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }

            SaveConfig("SqlInsertStatment", null);
        }

        [SetUp]
        public void SetUp()
        {
            SaveConfig("SqlInsertStatment", null);
        }

        
        [Test]
        [Ignore]
        public void GivenASqlInsertCommandIsInTheConfigurationFile_ThenUseThisValueForInsert()
        {
            var expected = "print('hi momm')";
            SaveConfig("SqlInsertStatment", expected);
            
            this.classUnderTest = new InsertServerRequestLogCommandFixture("RSAudit","select 1");

            var actual = this.classUnderTest.GetBaseInsertCommandSql();

            Assert.AreEqual(expected, actual);
            SaveConfig("SqlInsertStatment", null);
        }

        private void SaveConfig(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Path.Combine(AssemblyDirectory, Assembly.GetExecutingAssembly().ManifestModule.Name));
            var applicationSectionGroup = config.GetSectionGroup("applicationSettings");
            var applicationConfigSection = applicationSectionGroup.Sections["AsyncLogging.Properties.Settings"];
            var clientSection = (ClientSettingsSection)applicationConfigSection;

            var applicationSetting = clientSection.Settings.Get(key);
            
            applicationSetting.Value.ValueXml.InnerText = value;    
            
            applicationConfigSection.SectionInformation.ForceSave = true;

            config.Save();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("applicationSettings/AsyncLogging.Properties.Settings");
            ConfigurationManager.RefreshSection("AppSettings");
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private SqlConnection GetSqlConnection(string connectionName)
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
