
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
    public class InsertServerRequestLogCommandSqlConifgTests : BaseTest

    {
        private InsertServerRequestLogCommandFixture classUnderTest;
        
        [Test]
        public void GivenASqlInsertCommandIsInTheConfigurationFile_ThenUseThisValueForInsert()
        {
            var orgValue = AsyncConfig.SqlInsertStatement;
            var expected = "print('hi momm')";
            AsyncConfig.Settings.TryUpdate("SqlInsertStatement", expected, AsyncConfig.SqlInsertStatement);
            
            this.classUnderTest = new InsertServerRequestLogCommandFixture("RSAudit","select 1");

            var actual = this.classUnderTest.GetBaseInsertCommandSql();

            Assert.AreEqual(expected, actual);

            AsyncConfig.Settings.TryUpdate("SqlInsertStatement", orgValue, AsyncConfig.SqlInsertStatement);
        }        
    }
}
