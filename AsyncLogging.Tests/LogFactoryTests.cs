namespace AsyncLogging.Tests
{
    using System;

    using AsyncLogging.Enums;
    using AsyncLogging.Loggers;

    using NUnit.Framework;

    [TestFixture]
    public class LogFactoryTests : BaseTest
    {
        [Test]
        [TestCase("sqlserver")]
        [TestCase("SqlServer")]
        [TestCase("sql")]
        [TestCase("Sql")]
        [TestCase("File")]
        public void GivenIHaveATypePassedToMake_ThenIShouldGetAnInstanceOfThatTypeBack(string type)
        {
            var loggerType = (LoggerType)Enum.Parse(typeof(LoggerType), type, true);
            var actual = LogFactory.Make(loggerType);
            var expected = LogFactory.GetInstance(loggerType);
            Assert.AreEqual(actual.GetType(), expected.GetType());
        }
    }
}
