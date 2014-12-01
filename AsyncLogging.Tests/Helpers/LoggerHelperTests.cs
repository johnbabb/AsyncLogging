
namespace AsyncLogging.Tests.Helpers
{
    using System.IO;
    using System.Text;

    using AsyncLogging.Filters;
    using AsyncLogging.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class LoggerHelperTests
    {
        [Test]
        public void GivenANullObject_ThenGetOutputFilterStreamContents_ShouldReturnNull()
        {
            string expected = null;
            var actual = LoggerHelper.GetOutputFilterStreamContents(null);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenAFilterObject_ThenGetOutputFilterStreamContents_ShouldNotReturnNull()
        {
            string expected = string.Empty;
            var actual = LoggerHelper.GetOutputFilterStreamContents(new OutputFilterStream(new MemoryStream()));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(500, "*")]
        [TestCase(500, ".*")]
        [TestCase(500, "500")]
        [TestCase(500, "200|500|100")]
        [TestCase(500, "200|300|500")]
        [TestCase(500, "500|300|100")]
        public void GivenStatusCode_ThenIsLoggingStatusCode_ShouldReturnTrue(int statusCode, string match)
        {
            var expected = true;

            var actual = LoggerHelper.IsLoggingStatusCode(new System.Web.HttpResponse(new StreamWriter(new MemoryStream())) { StatusCode = statusCode }, match);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(500, "400")]
        [TestCase(500, "200|100")]
        [TestCase(500, "200|300|400")]
        [TestCase(500, "300|(*")]
        public void GivenStatusCode_ThenIsLoggingStatusCode_ShouldReturnFalse(int statusCode, string match)
        {
            var expected = false;

            var actual = LoggerHelper.IsLoggingStatusCode(new System.Web.HttpResponse(new StreamWriter(new MemoryStream())) { StatusCode = statusCode }, match);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("json", "*")]
        [TestCase("application/json", "application/json")]
        [TestCase("application/json", "application/json|application/xml")]
        [TestCase("application/json", "application/asp|application/json|application/xml")]
        [TestCase("application/json", "application/asp|application/xml|application/json")]
        [TestCase("application/json", "json|xml|html|text")]
        public void GivenContentType_ThenIsLoggingContentType_ShouldReturnTrue(string contentType, string match)
        {
            var expected = true;

            var actual = LoggerHelper.IsLoggingContentType(new System.Web.HttpRequest("","http://google.com","") { ContentType = contentType }, match);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("json", "**")]
        [TestCase("application/json", "application/jsons")]
        [TestCase("application/json", "application/jsons|application/xml")]
        [TestCase("application/json", "application/asp|application/jsons|application/xml")]
        public void GivenContentType_ThenIsLoggingContentType_ShouldReturnFalse(string contentType, string match)
        {
            var expected = false;

            var actual = LoggerHelper.IsLoggingContentType(new System.Web.HttpRequest("", "http://google.com", "") { ContentType = contentType }, match);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GvienEmptyRequest_ThenGetDocumentContents_ShouldNotError()
        {
            var expected = string.Empty;
            var actual = LoggerHelper.GetDocumentContents(new System.Web.HttpRequest("", "http://google.com", ""){ ContentEncoding = Encoding.ASCII});
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void GvienNullRequest_ThenGetDocumentContents_ShouldNotError()
        {
            var expected = string.Empty;
            var actual = LoggerHelper.GetDocumentContents(null);
            Assert.AreEqual(expected, actual);

        }
        
    }
}
