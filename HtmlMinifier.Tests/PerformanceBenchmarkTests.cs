using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class PerformanceBenchmarkTests
    {
        readonly Features noFeatures = new Features(new List<string>().ToArray());
        private const int PERFORMANCE_THRESHOLD_MS = 2000; // 1 second threshold for most tests

        [TestMethod]
        public void Performance_SmallDocument_ShouldCompleteQuickly()
        {
            // Arrange
            string html = "<html><head><title>Test</title></head><body><div>Content</div></body></html>";
            Stopwatch sw = new Stopwatch();

            // Act
            sw.Start();
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
            sw.Stop();

            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds < 100, $"Small document took {sw.ElapsedMilliseconds}ms (expected < 100ms)");
            Assert.IsTrue(result.Length > 0);
            Console.WriteLine($"Small document minification: {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void Performance_DocumentWithManyComments_ShouldComplete()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><body>");
            
            for (int i = 0; i < 500; i++)
            {
                sb.Append($"<!-- Comment {i} -->");
                sb.Append($"<div>Content {i}</div>");
            }
            
            sb.Append("</body></html>");
            string html = sb.ToString();
            Stopwatch sw = new Stopwatch();

            // Act
            sw.Start();
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
            sw.Stop();

            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds < PERFORMANCE_THRESHOLD_MS * 2, 
                $"Document with many comments took {sw.ElapsedMilliseconds}ms (expected < {PERFORMANCE_THRESHOLD_MS * 2}ms)");
            Assert.IsTrue(result.Length > 0);
            Assert.IsFalse(result.Contains("<!-- Comment"));
            Console.WriteLine($"Document with many comments: {sw.ElapsedMilliseconds}ms - Original: {html.Length} bytes, Minified: {result.Length} bytes");
        }

        [TestMethod]
        public void Performance_DocumentWithManyScripts_ShouldComplete()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><body>");
            
            for (int i = 0; i < 100; i++)
            {
                sb.Append($"<script>// Comment {i}\nvar x{i} = {i}; console.log(x{i});</script>");
                sb.Append($"<div>Content {i}</div>");
            }
            
            sb.Append("</body></html>");
            string html = sb.ToString();
            Stopwatch sw = new Stopwatch();

            // Act
            sw.Start();
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
            sw.Stop();

            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds < PERFORMANCE_THRESHOLD_MS * 2, 
                $"Document with many scripts took {sw.ElapsedMilliseconds}ms (expected < {PERFORMANCE_THRESHOLD_MS * 2}ms)");
            Assert.IsTrue(result.Length > 0);
            Console.WriteLine($"Document with many scripts: {sw.ElapsedMilliseconds}ms - Original: {html.Length} bytes, Minified: {result.Length} bytes");
        }

        [TestMethod]
        public void Performance_DocumentWithDeepNesting_ShouldComplete()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            int depth = 100;
            
            for (int i = 0; i < depth; i++)
            {
                sb.Append($"<div class=\"level-{i}\">");
            }
            
            sb.Append("<p>Deeply nested content</p>");
            
            for (int i = 0; i < depth; i++)
            {
                sb.Append("</div>");
            }
            
            string html = sb.ToString();
            Stopwatch sw = new Stopwatch();

            // Act
            sw.Start();
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
            sw.Stop();

            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds < PERFORMANCE_THRESHOLD_MS, 
                $"Deeply nested document took {sw.ElapsedMilliseconds}ms (expected < {PERFORMANCE_THRESHOLD_MS}ms)");
            Assert.IsTrue(result.Length > 0);
            Console.WriteLine($"Deeply nested document: {sw.ElapsedMilliseconds}ms - Original: {html.Length} bytes, Minified: {result.Length} bytes");
        }

        [TestMethod]
        public void Performance_MultipleRuns_ShouldBeConsistent()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 500; i++)
            {
                sb.Append($"<div class=\"item\">{i}</div>");
            }
            string html = sb.ToString();
            
            List<long> times = new List<long>();

            // Act - Run 10 times
            for (int run = 0; run < 10; run++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
                sw.Stop();
                times.Add(sw.ElapsedMilliseconds);
            }

            // Assert
            double averageTime = 0;
            foreach (var time in times)
            {
                averageTime += time;
            }
            averageTime /= times.Count;

            Assert.IsTrue(averageTime < PERFORMANCE_THRESHOLD_MS, 
                $"Average time {averageTime}ms exceeded threshold {PERFORMANCE_THRESHOLD_MS}ms");
            
            Console.WriteLine($"Multiple runs statistics:");
            Console.WriteLine($"  Average: {averageTime:F2}ms");
            Console.WriteLine($"  Min: {times[0]}ms");
            Console.WriteLine($"  Max: {times[times.Count - 1]}ms");
        }

        [TestMethod]
        public void Performance_CompressionRatio_SmallDocument()
        {
            // Arrange
            string html = @"<!DOCTYPE html>
<html>
<head>
    <title>Test Page</title>
</head>
<body>
    <div class=""container"">
        <h1>Hello World</h1>
        <p>This is a test.</p>
    </div>
</body>
</html>";

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            int originalSize = html.Length;
            int minifiedSize = result.Length;
            double compressionRatio = (originalSize - minifiedSize) * 100.0 / originalSize;
            
            Assert.IsTrue(compressionRatio > 10, "Compression ratio should be at least 10%");
            Console.WriteLine($"Small document compression: {compressionRatio:F2}% (Original: {originalSize} bytes, Minified: {minifiedSize} bytes)");
        }

        [TestMethod]
        public void Performance_CompressionRatio_DocumentWithWhitespace()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append($"    <div class=\"item\">    \n");
                sb.Append($"        <h2>    Title {i}    </h2>    \n");
                sb.Append($"        <p>    Content {i}    </p>    \n");
                sb.Append($"    </div>    \n");
            }
            string html = sb.ToString();

            // Act
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);

            // Assert
            int originalSize = html.Length;
            int minifiedSize = result.Length;
            double compressionRatio = (originalSize - minifiedSize) * 100.0 / originalSize;
            
            Assert.IsTrue(compressionRatio > 20, "Compression ratio should be at least 20% for whitespace-heavy documents");
            Console.WriteLine($"Whitespace-heavy document compression: {compressionRatio:F2}% (Original: {originalSize} bytes, Minified: {minifiedSize} bytes)");
        }

        [TestMethod]
        public void Performance_RazorSyntax_ShouldComplete()
        {
            // Arrange
            StringBuilder sb = new StringBuilder();
            sb.Append("@model List<Item>\n");
            for (int i = 0; i < 100; i++)
            {
                sb.Append($"<div>@Html.DisplayFor(m => m[{i}].Name)</div>\n");
            }
            string html = sb.ToString();
            Stopwatch sw = new Stopwatch();

            // Act
            sw.Start();
            string result = StreamReaderExtension.MinifyHtmlCode(html, noFeatures);
            sw.Stop();

            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds < PERFORMANCE_THRESHOLD_MS, 
                $"Razor syntax document took {sw.ElapsedMilliseconds}ms (expected < {PERFORMANCE_THRESHOLD_MS}ms)");
            Assert.IsTrue(result.Contains("@model"));
            Console.WriteLine($"Razor syntax document: {sw.ElapsedMilliseconds}ms");
        }
    }
}
