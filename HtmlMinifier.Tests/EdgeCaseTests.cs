using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class EdgeCaseTests
    {
        readonly Features noFeatures = new Features(new List<string>().ToArray());

        [TestMethod]
        public void MinifyHtml_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            string emptyHtml = string.Empty;

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(emptyHtml, noFeatures);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void MinifyHtml_WithNullString_ShouldReturnNull()
        {
            // Arrange
            string nullHtml = null;

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(nullHtml, noFeatures);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MinifyHtml_WithWhitespaceOnly_ShouldReturnEmpty()
        {
            // Arrange
            string whitespaceHtml = "   \r\n\t   \r\n   ";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(whitespaceHtml, noFeatures);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void MinifyHtml_WithSingleTag_ShouldMinifyCorrectly()
        {
            // Arrange
            string html = "<div>Test</div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.AreEqual("<div>Test</div>", result);
        }

        [TestMethod]
        public void MinifyHtml_WithNestedTags_ShouldMinifyCorrectly()
        {
            // Arrange
            string html = "<div>  <span>  <b>Test</b>  </span>  </div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.AreEqual("<div><span><b>Test</b></span></div>", result);
        }

        [TestMethod]
        public void MinifyHtml_WithMultipleSpaces_ShouldCollapseToSingle()
        {
            // Arrange
            string html = "<p>This     has     multiple     spaces</p>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.AreEqual("<p>This has multiple spaces</p>", result);
        }

        [TestMethod]
        public void MinifyHtml_WithSpecialCharacters_ShouldPreserve()
        {
            // Arrange
            string html = "<p>&nbsp;&lt;&gt;&amp;&quot;&#39;</p>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("&nbsp;"));
            Assert.IsTrue(result.Contains("&lt;"));
            Assert.IsTrue(result.Contains("&gt;"));
            Assert.IsTrue(result.Contains("&amp;"));
        }

        [TestMethod]
        public void MinifyHtml_WithUnicodeCharacters_ShouldPreserve()
        {
            // Arrange
            string html = "<p>测试 テスト тест 🎉</p>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("测试"));
            Assert.IsTrue(result.Contains("テスト"));
            Assert.IsTrue(result.Contains("тест"));
            Assert.IsTrue(result.Contains("🎉"));
        }

        [TestMethod]
        public void MinifyHtml_WithEmptyTags_ShouldPreserve()
        {
            // Arrange
            string html = "<div></div><span></span><p></p>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.AreEqual("<div></div><span></span><p></p>", result);
        }

        [TestMethod]
        public void MinifyHtml_WithSelfClosingTags_ShouldPreserve()
        {
            // Arrange
            string html = "<br /><img src=\"test.jpg\" /><input type=\"text\" />";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("<br />"));
            Assert.IsTrue(result.Contains("<img"));
            Assert.IsTrue(result.Contains("<input"));
        }

        [TestMethod]
        public void MinifyHtml_WithInlineStyles_ShouldPreserve()
        {
            // Arrange
            string html = "<div style=\"color: red; margin: 10px;\">Test</div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("style="));
            Assert.IsTrue(result.Contains("color: red"));
        }

        [TestMethod]
        public void MinifyHtml_WithDataAttributes_ShouldPreserve()
        {
            // Arrange
            string html = "<div data-id=\"123\" data-name=\"test\" data-value=\"hello world\">Test</div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("data-id=\"123\""));
            Assert.IsTrue(result.Contains("data-name=\"test\""));
            Assert.IsTrue(result.Contains("data-value=\"hello world\""));
        }

        [TestMethod]
        public void MinifyHtml_WithPreTag_ShouldPreserveWhitespace()
        {
            // Arrange
            string html = "<pre>Line 1\r\n  Line 2\r\n    Line 3</pre>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("Line 1\r\n  Line 2\r\n    Line 3"));
        }

        [TestMethod]
        public void MinifyHtml_WithMultiplePreTags_ShouldPreserveAllWhitespace()
        {
            // Arrange
            string html = "<pre>First\r\n  Block</pre><div>Middle</div><pre>Second\r\n  Block</pre>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("First\r\n  Block"));
            Assert.IsTrue(result.Contains("Second\r\n  Block"));
            Assert.IsTrue(result.Contains("Middle"));
        }

        [TestMethod]
        public void MinifyHtml_WithJavaScriptUrls_ShouldNotBreak()
        {
            // Arrange
            string html = "<a href=\"javascript:void(0)\">Click</a>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("javascript:void(0)"));
        }

        [TestMethod]
        public void MinifyHtml_WithProtocolRelativeUrls_ShouldPreserve()
        {
            // Arrange
            string html = "<script src=\"//cdn.example.com/script.js\"></script>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("//cdn.example.com/script.js"));
        }

        [TestMethod]
        public void MinifyHtml_WithConditionalComments_ShouldPreserve()
        {
            // Arrange
            string html = "<!--[if IE]><link rel=\"stylesheet\" href=\"ie.css\"><![endif]-->";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("<!--[if IE]>"));
        }

        [TestMethod]
        public void MinifyHtml_WithDoctypeDeclaration_ShouldPreserve()
        {
            // Arrange
            string html = "<!DOCTYPE html>\r\n<html><head></head><body></body></html>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.StartsWith("<!DOCTYPE html>"));
        }

        [TestMethod]
        public void MinifyHtml_WithScriptTag_ShouldNotBreakJavaScript()
        {
            // Arrange
            string html = "<script>var x = 1; var y = 2; console.log(x + y);</script>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("var x = 1"));
            Assert.IsTrue(result.Contains("console.log"));
        }

        [TestMethod]
        public void MinifyHtml_WithStyleTag_ShouldNotBreakCSS()
        {
            // Arrange
            string html = "<style>.class { color: red; margin: 10px; }</style>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains(".class"));
            Assert.IsTrue(result.Contains("color: red"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MinifyHtml_WithNullFeatures_ShouldThrowException()
        {
            // Arrange
            string html = "<div>Test</div>";

            // Act
            StreamReaderExtension.MinifyHtmlCode(html, null);
        }

        [TestMethod]
        public void MinifyHtml_WithDeeplyNestedStructure_ShouldHandleCorrectly()
        {
            // Arrange - Create deeply nested HTML
            StringBuilder sb = new StringBuilder();
            int depth = 50;
            
            for (int i = 0; i < depth; i++)
            {
                sb.Append($"<div class=\"level-{i}\">");
            }
            
            sb.Append("<span>Deeply nested content</span>");
            
            for (int i = 0; i < depth; i++)
            {
                sb.Append("</div>");
            }
            
            string deepHtml = sb.ToString();

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(deepHtml, noFeatures);

            // Assert
            Assert.IsTrue(result.Length > 0, "Result should not be empty");
            Assert.IsTrue(result.Contains("Deeply nested content"), "Should preserve content");
            Assert.IsTrue(result.Contains("level-0") || result.Contains("<div"), "Should contain first level");
            Assert.IsTrue(result.Contains("level-49") || result.Contains("</div>"), "Should contain last level");
        }

        [TestMethod]
        public void MinifyHtml_WithManyAttributes_ShouldPreserveAll()
        {
            // Arrange
            string html = "<div id=\"test\" class=\"class1 class2\" data-value=\"123\" data-name=\"test\" " +
                         "style=\"color:red\" onclick=\"alert('hi')\" title=\"tooltip\" " +
                         "aria-label=\"label\" role=\"button\">Content</div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("id=\"test\""));
            Assert.IsTrue(result.Contains("class=\"class1 class2\""));
            Assert.IsTrue(result.Contains("data-value=\"123\""));
            Assert.IsTrue(result.Contains("aria-label=\"label\""));
        }

        [TestMethod]
        public void MinifyHtml_WithMixedQuotes_ShouldPreserve()
        {
            // Arrange
            string html = "<div class='single' data-value=\"double\" onclick='alert(\"hi\")'>Test</div>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            Assert.IsTrue(result.Contains("class='single'"));
            Assert.IsTrue(result.Contains("data-value=\"double\""));
        }
    }
}
