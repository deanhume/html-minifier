using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class FileExtensionTests
    {
        [TestMethod]
        public void GithubIssue25_ShouldReturnCorrectly()
        {
            Assert.IsTrue("test.html".IsHtmlFile());
            Assert.IsTrue("codes.js.aspx".IsHtmlFile());
            Assert.IsTrue("test.inc".IsHtmlFile());

            Assert.IsFalse("codes.aspx.js".IsHtmlFile());
            Assert.IsFalse("aspx.codes.js".IsHtmlFile());
        }
    }
}
