
namespace AsyncLogging.Tests.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    using AsyncLogging.Loggers;
    using AsyncLogging.SqlCommands;

    using NUnit.Framework;

    [TestFixture]
    public class InsertServerRequestLogCommandTests : BaseTest
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
        }

        [SetUp]
        public void SetUp()
        {            
        }

        [Test]
        public void GivenAInvalidConnectionName_ThenTheInsertCommandWillFailGracefully()
        {
            var expected = 0;
            this.classUnderTest = new InsertServerRequestLogCommandFixture("BlahBlahConnectionName", "print '1';");
            var actual = this.classUnderTest.Execute(new ServerRequestLog());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("Server=.;Database=RS_DEV;Trusted_Connection=True;MultipleActiveResultSets=true;")]
        [TestCase("RSAudit")]
        public void GivenAValidConnectionStringOrName_ThenTheInsertCommandWillExecute(string connectionNameOrString)
        {
            var expected = 1;
            
            var insertCommandSql = @"
                    Select @RequestDate 'RequestDate'
                        ,@RequestDateInTicks 'RequestDateInTicks'
                        ,@RequestBy 'RequestBy'
                        ,@RequestMethod 'RequestMethod'
                        ,@RequestUrl 'RequestUrl'
                        ,@RequestBody 'RequestBody'
                        ,@ResponseCode 'ResponseCode'
                        ,@ResponseBody 'ResponseBody'
                        ,@Host 'Host'
                        ,@CreatedOn 'CreatedOn'
                     into #temptable; Drop table #temptable";

            this.classUnderTest = new InsertServerRequestLogCommandFixture(connectionNameOrString, insertCommandSql);
            
            var log = new ServerRequestLog()
                          {
                              Host = "host",
                              RequestBody = "body",
                              RequestBy = "jxb15",
                              RequestDate = this.CurrentDate,
                              RequestDateInTicks = this.CurrentDate.Ticks,
                              RequestMethod = "GET",
                              RequestUrl = "URL",
                              ResponseBody = "body",
                              ResponseCode = 200
                          };

            var actual = this.classUnderTest.Execute(log);
            Assert.AreEqual(expected, actual);
        }       

        [Test]
        public void GivenAValidServerRequestLogObject_ThenTheInsertCommandWillInsert()
        {
            var expected = 1;

            this.classUnderTest = new InsertServerRequestLogCommandFixture();

            var log = new ServerRequestLog()
            {
                Host = "host",
                RequestBody = "body",
                RequestBy = "jxb15",
                RequestDate = this.CurrentDate,
                RequestDateInTicks = this.CurrentDate.Ticks,
                RequestMethod = "GET",
                RequestUrl = "URL",
                ResponseBody = "body",
                ResponseCode = 200
            };

            var actual = this.classUnderTest.Execute(log);
            Assert.AreEqual(expected, actual);
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
