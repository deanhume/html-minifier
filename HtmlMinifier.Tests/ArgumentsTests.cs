using NUnit.Framework;
using System.Collections.Generic;

namespace HtmlMinifier.Tests
{
    [TestFixture]
    public class ArgumentsTests
    {
        [Test]
        public void FindValuesInArgs_WithIgnores_ShouldReturnCorrectly()
        {
            // Arrange
            List<string> argsList = new List<string>();
            argsList.Add("ignorehtmlcomments");
            argsList.Add("ignorejscomments");

            // Act
            Features disabledFeatures = new Features(argsList.ToArray());

            // Assert
            Assert.That(disabledFeatures.IgnoreHtmlComments, Is.True);
            Assert.That(disabledFeatures.IgnoreJsComments, Is.True);
        }

        [Test]
        public void FindValuesInArgs_WithOneIgnore_ShouldReturnCorrectly()
        {
            // Arrange
            List<string> argsList = new List<string>();
            argsList.Add("ignorehtmlcomments");

            // Act
            Features disabledFeatures = new Features(argsList.ToArray());

            // Assert
            Assert.That(disabledFeatures.IgnoreHtmlComments, Is.True);
            Assert.That(disabledFeatures.IgnoreJsComments, Is.False);
        }
    }
}
