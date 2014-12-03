
namespace AsyncLogging.Tests.SqlCommands
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml;

    using AsyncLogging.Extensions;
    using AsyncLogging.Filters;
    using AsyncLogging.Helpers;
    using AsyncLogging.Loggers;
    using AsyncLogging.SqlCommands;

    using NUnit.Framework;

    [TestFixture]
    public class InsertServerRequestLogCommandTests : BaseTest
    {
        private InsertServerRequestLogCommandFixture classUnderTest;

        private DateTime CurrentDate;

        private string OrgSqlInsertStatement = AsyncConfig.SqlInsertStatement;

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
                var command = new SqlCommand("delete from ServerRequestLogs where RequestDate >= '" + this.CurrentDate + "'", connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        [SetUp]
        public void SetUp()
        {
            OrgSqlInsertStatement = AsyncConfig.SqlInsertStatement;
            SetSqlInsertStatement(string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            SetSqlInsertStatement(OrgSqlInsertStatement);
        }

        [Test]
        public void GivenASqlInsertCommandIsInTheConfigurationFile_ThenUseThisValueForInsert()
        {
            var orgValue = AsyncConfig.SqlInsertStatement;
            var expected = "print('hi momm')";
            AsyncConfig.Settings.TryUpdate("SqlInsertStatement", expected, AsyncConfig.SqlInsertStatement);

            this.classUnderTest = new InsertServerRequestLogCommandFixture("RSAudit", "select 1");

            var actual = this.classUnderTest.GetBaseInsertCommandSql();

            Assert.AreEqual(expected, actual);

            AsyncConfig.Settings.TryUpdate("SqlInsertStatement", orgValue, AsyncConfig.SqlInsertStatement);
        }  

        [Test]
        [TestCase("RSAudit")]
        public void GivenAValidConnectionName_ThenTheInsertCommandWillExecute(string connectionNameOrString)
        {
            var expected = 1;

            var insertCommandSql = @"
                    Select @RequestDate 'RequestDate'
                        ,@RequestBy 'RequestBy'
                        ,@RequestMethod 'RequestMethod'
                        ,@RequestUrl 'RequestUrl'
                        ,@RequestBody 'RequestBody'
                        ,@ResponseCode 'ResponseCode'
                        ,@ResponseBody 'ResponseBody'
                        ,@Host 'Host'
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
        [TestCase("aaaSteve")]
        public void GivenAnInvalidConnectionName_ThenTheInsertCommandWillThrowException(string connectionNameOrString)
        {
            var expected = 1;

            var insertCommandSql = @"
                    Select @RequestDate 'RequestDate'
                        ,@RequestBy 'RequestBy'
                        ,@RequestMethod 'RequestMethod'
                        ,@RequestUrl 'RequestUrl'
                        ,@RequestBody 'RequestBody'
                        ,@ResponseCode 'ResponseCode'
                        ,@ResponseBody 'ResponseBody'
                        ,@Host 'Host'
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
            var ex = Assert.Throws<ArgumentException>(() => this.classUnderTest.Execute(log));
            Assert.True(ex.Message.Contains(connectionNameOrString));
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
 
        [Test]
        public void GivenAValidCommand_ThenInsertAsync_ShouldReturnOneRowAffected()
        {
            var expected = 1;
            var actual = 0;
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
            AsyncCallback cb = ar => { };
            var state = new object();

            var result = this.classUnderTest.BeginExecuteNonQuery(cb, state, log) as Task<int>;
            this.classUnderTest.EndExecuteNonQuery(result);
            actual = result.Result;
            Assert.AreEqual(expected, actual);

            actual = this.classUnderTest.GetRowCount();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenABadConnection_ThenInsertAsync_ShouldThorwArgumentException()
        {
            var expected = -1;
            var actual = 0;
            this.classUnderTest = new InsertServerRequestLogCommandFixture("abc123","abc123");
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
            
            AsyncCallback cb = ar => { };
            var state = new object();
            Task<int> result = Task<int>.Factory.StartNew(() => -1);
            

            var ex = Assert.Throws<ArgumentException>(() => result = this.classUnderTest.BeginExecuteNonQuery(cb, state, log) as Task<int>);
        }

        [Test]
        public void GivenBadData_ThenInsertAsync_ShouldThorwAggregateException()
        {
            var expected = -1;
            var actual = 0;
            this.classUnderTest = new InsertServerRequestLogCommandFixture();
            var log = new ServerRequestLog();
            //{
            //    Host = "host",
            //    RequestBody = "body",
            //    RequestBy = "jxb15",
            //    RequestDate = this.CurrentDate,
            //    RequestDateInTicks = this.CurrentDate.Ticks,
            //    RequestMethod = "GET",
            //    RequestUrl = "URL",
            //    ResponseBody = "body",
            //    ResponseCode = 200
            //};
            Task<int> result = Task<int>.Factory.StartNew(() => -1);
            AsyncCallback cb = ar => { };
            var state = new object();
            result = this.classUnderTest.BeginExecuteNonQuery(cb, state, log) as Task<int>;
            var ex = Assert.Throws<AggregateException>(() => this.classUnderTest.EndExecuteNonQuery(result));
                      
        }

        private void SetSqlInsertStatement(string value)
        {
            AsyncConfig.Settings.TryUpdate("SqlInsertStatement", value, AsyncConfig.SqlInsertStatement);
        }
    }
}
