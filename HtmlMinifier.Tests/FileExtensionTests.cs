using NUnit.Framework;
using System.Collections.Generic;

namespace HtmlMinifier.Tests
{
    [TestFixture]
    public class FileExtensionTests
    {
        [Test]
        public void GithubIssue25__ShouldReturnCorrectly()
        {                        
            Assert.That("test.html".IsHtmlFile(), Is.True);
            Assert.That("codes.js.aspx".IsHtmlFile(), Is.True);

            Assert.That("codes.aspx.js".IsHtmlFile(), Is.False);
            Assert.That("aspx.codes.js".IsHtmlFile(), Is.False);
        }
    }
}
