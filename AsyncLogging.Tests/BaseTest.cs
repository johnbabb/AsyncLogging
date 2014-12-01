namespace AsyncLogging.Tests
{
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
    }
}