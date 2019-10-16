using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class ArgumentsTests
    {
        [TestMethod]
        public void FindValuesInArgs_WithIgnores_ShouldReturnCorrectly()
        {
            // Arrange
            List<string> argsList = new List<string>();
            argsList.Add("ignorehtmlcomments");
            argsList.Add("ignorejscomments");
            argsList.Add("ignoreknockoutcomments");

            // Act
            Features disabledFeatures = new Features(argsList.ToArray());

            // Assert
            Assert.IsTrue(disabledFeatures.IgnoreHtmlComments);
            Assert.IsTrue(disabledFeatures.IgnoreJsComments);
            Assert.IsTrue(disabledFeatures.IgnoreKnockoutComments);
        }

        [TestMethod]
        public void FindValuesInArgs_WithOneIgnore_ShouldReturnCorrectly()
        {
            // Arrange
            List<string> argsList = new List<string>();
            argsList.Add("ignorehtmlcomments");

            // Act
            Features disabledFeatures = new Features(argsList.ToArray());

            // Assert
            Assert.IsTrue(disabledFeatures.IgnoreHtmlComments);
            Assert.IsFalse(disabledFeatures.IgnoreJsComments);
            Assert.IsFalse(disabledFeatures.IgnoreKnockoutComments);
        }
    }
}
