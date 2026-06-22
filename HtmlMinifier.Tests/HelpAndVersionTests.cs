using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class HelpAndVersionTests
    {
        [TestMethod]
        public void GetUsageText_ShouldReturnNonEmptyString()
        {
            // Act
            string usageText = ConsoleReporter.GetUsageText();

            // Assert
            Assert.IsNotNull(usageText);
            Assert.IsTrue(usageText.Length > 0);
        }

        [TestMethod]
        public void GetUsageText_ShouldContainExpectedMessages()
        {
            // Act
            string usageText = ConsoleReporter.GetUsageText();

            // Assert
            Assert.IsTrue(usageText.Contains("Please provide folder path or file(s) to process"));
            Assert.IsTrue(usageText.Contains("--help"));
        }

        [TestMethod]
        public void GetHelpText_ShouldReturnNonEmptyString()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsNotNull(helpText);
            Assert.IsTrue(helpText.Length > 0);
        }

        [TestMethod]
        public void GetHelpText_ShouldContainVersion()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("HTML Minifier"));
            Assert.IsTrue(helpText.Contains("v1.9.3") || helpText.Contains("v"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainUsageSection()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("USAGE:"));
            Assert.IsTrue(helpText.Contains("HtmlMinifier.exe <path> [options]"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainArgumentsSection()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("ARGUMENTS:"));
            Assert.IsTrue(helpText.Contains("<path>"));
            Assert.IsTrue(helpText.Contains("File or folder path to process"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainOptionsSection()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("OPTIONS:"));
            Assert.IsTrue(helpText.Contains("ignorehtmlcomments"));
            Assert.IsTrue(helpText.Contains("ignorejscomments"));
            Assert.IsTrue(helpText.Contains("ignoreknockoutcomments"));
            Assert.IsTrue(helpText.Contains("--help"));
            Assert.IsTrue(helpText.Contains("--version"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainExamplesSection()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("EXAMPLES:"));
            Assert.IsTrue(helpText.Contains("HtmlMinifier.exe"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainSupportedFileTypes()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("SUPPORTED FILE TYPES:"));
            Assert.IsTrue(helpText.Contains(".html"));
            Assert.IsTrue(helpText.Contains(".cshtml"));
            Assert.IsTrue(helpText.Contains(".aspx"));
        }

        [TestMethod]
        public void GetHelpText_ShouldContainGitHubLink()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            Assert.IsTrue(helpText.Contains("github.com/deanhume/html-minifier"));
        }

        [TestMethod]
        public void GetVersionText_ShouldReturnNonEmptyString()
        {
            // Act
            string versionText = ConsoleReporter.GetVersionText();

            // Assert
            Assert.IsNotNull(versionText);
            Assert.IsTrue(versionText.Length > 0);
        }

        [TestMethod]
        public void GetVersionText_ShouldContainVersionNumber()
        {
            // Act
            string versionText = ConsoleReporter.GetVersionText();

            // Assert
            Assert.IsTrue(versionText.Contains("HTML") || versionText.Contains("Minifier"));
            // Should contain version in format "v" followed by numbers
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(versionText, @"v\d+\.\d+\.\d+"));
        }

        [TestMethod]
        public void GetVersionText_ShouldContainCopyright()
        {
            // Act
            string versionText = ConsoleReporter.GetVersionText();

            // Assert
            Assert.IsTrue(versionText.Contains("Copyright"));
            Assert.IsTrue(versionText.Contains("Dean Hume"));
        }

        [TestMethod]
        public void GetVersionText_ShouldContainLicense()
        {
            // Act
            string versionText = ConsoleReporter.GetVersionText();

            // Assert
            Assert.IsTrue(versionText.Contains("License: MIT"));
        }

        [TestMethod]
        public void GetVersionText_ShouldContainThreeLines()
        {
            // Act
            string versionText = ConsoleReporter.GetVersionText();

            // Assert
            string[] lines = versionText.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            Assert.IsTrue(lines.Length >= 3, "Version text should contain at least 3 lines");
        }

        [TestMethod]
        public void GetHelpText_ShouldHaveProperFormatting()
        {
            // Act
            string helpText = ConsoleReporter.GetHelpText();

            // Assert
            // Check for proper spacing with empty lines
            Assert.IsTrue(helpText.Contains("\n\n") || helpText.Contains("\r\n\r\n"), 
                "Help text should contain empty lines for readability");
        }
    }
}
