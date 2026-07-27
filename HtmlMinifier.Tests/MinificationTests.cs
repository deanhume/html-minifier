using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtmlMinifier.Tests
{
    [TestClass]
    public class MinificationTests
    {
        
        readonly string _testDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Data");
        readonly Features noFeatures = new Features(new List<string>().ToArray());

        [TestMethod]
        public void ReadHtml_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.StandardResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.Standard, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void MinifyContents_WithComments_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.CommentsResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.Comments, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void MinifyContents_WithModelList_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.ModelListResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.ModelList, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void MinifyContents_WithLanguageSpecficCharacters_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.LanguageSpecificCharactersResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.LanguageSpecificCharacters, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue10_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/10
            // Arrange
            string expectedResult = DataHelpers.GithubIssue10Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue10, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue13_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/13
            string expectedResult = DataHelpers.GithubIssue13Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue13, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void SixtyFiveKCharacters_ShouldBreakToNextLine()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14
            List<string> args = new List<string> { "pathToFiles", "60000" };

            string expectedResult = DataHelpers.SixtyFiveThousandCharactersResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.SixtyFiveThousandCharacters, new Features(args.ToArray()));

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void SixtyFiveKCharacters_WithoutArgs_ShouldMakeNoChange()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14
            List<string> args = new List<string> { "pathToFiles" };

            string expectedResult = DataHelpers.SixtyFiveThousandCharactersNoBreakResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.SixtyFiveThousandCharacters, new Features(args.ToArray()));

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void RemoveJavaScriptComments_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.JavaScriptCommentsResult;

            // Act
            string removedComments = StreamReaderExtension.RemoveJavaScriptComments(DataHelpers.JavaScriptComments);

            // Assert
            Assert.AreEqual(removedComments, expectedResult);
        }

        [TestMethod]
        public void RemoveMultipleJavaScriptComments_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.MultipleJavaScriptCommentsResult;

            // Act
            string removedComments = StreamReaderExtension.RemoveJavaScriptComments(DataHelpers.MultipleJavaScriptComments);

            // Assert
            Assert.AreEqual(removedComments, expectedResult);
        }

        [TestMethod]
        public void GithubIssue19Inherits_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/19
            string expectedResult = DataHelpers.GithubIssue19InheritsResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue19Inherits, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue19Multiple_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/19
            string expectedResult = DataHelpers.GithubIssue19MultipleResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue19Multiple, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue23_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/23
            string expectedResult = DataHelpers.GithubIssue23Result;

            // test IgnoreHtmlComments
            List<string> args = new List<string> { "ignorehtmlcomments" };

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue23, new Features(args.ToArray()));

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue36_ShouldReturnCorrectly()
        {
            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue36, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, DataHelpers.GithubIssue36Result);
        }

        [TestMethod]
        public void GithubIssue38_IgnorePreTag_ShouldReturnCorrectly()
        {
            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue38, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, DataHelpers.GithubIssue38Result);
        }

        [TestMethod]
        public void RemoveMultipleHtmlComments_WithIncludeVirtuals_ShouldReturnCorrectly()
        {
            string expectedResult = DataHelpers.WithIncludeVirtualsResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.WithIncludeVirtuals, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void BadHTML_ShouldReturnCorrectly()
        {
            string badHtml = "@model .";
            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(badHtml, noFeatures);

            // Assert
            Assert.AreEqual(badHtml, badHtml);
        }

        [TestMethod]
        public void ModelViewDoubleLessThanSign_ShouldTakenToTop()
        {
            string expectedResult = DataHelpers.TupleModelExpectedResult;
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.TupleModel, noFeatures);
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void TextLineAtSign_ShouldReplaceWithTextTags()
        {
            string expectedResult = DataHelpers.WithAtSignTextExpectedResult;
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.WithAtSignText, noFeatures);
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void CommentLineWithTripleSlash_ShouldBeRemoved()
        {
            string expectedResult = DataHelpers.CommentLineWithTripleSlashExpectedResult;
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.CommentLineWithTripleSlash, noFeatures);
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void MinifyContents_WithArabic_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.ArabicResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.Arabic, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue44_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.GithubIssue44Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue44, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue54_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.GithubIssue54Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue54, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue47_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/47
            // Arrange
            string expectedResult = DataHelpers.GithubIssue47Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue47, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue30_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/30
            // Arrange
            string expectedResult = DataHelpers.GithubIssue30Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue30, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue30Complex_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/30
            // Arrange
            string expectedResult = DataHelpers.GithubIssue30ComplexResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue30Complex, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void TextareaTag_WhitespaceShouldBePreserved()
        {
            // Arrange
            string expectedResult = DataHelpers.TextareaProtectionResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.TextareaProtection, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void CodeTag_WhitespaceShouldBePreserved()
        {
            // Arrange
            string expectedResult = DataHelpers.CodeProtectionResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.CodeProtection, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void AttributeWhitespace_ShouldBeNormalised()
        {
            // Arrange
            string expectedResult = DataHelpers.AttributeWhitespaceResult;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.AttributeWhitespace, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue49_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/49
            // Arrange
            string expectedResult = DataHelpers.GithubIssue49Result;

            // Act
            string minifiedHtml = StreamReaderExtension.MinifyHtmlCode(DataHelpers.GithubIssue49, noFeatures);

            // Assert
            Assert.AreEqual(minifiedHtml, expectedResult);
        }

        [TestMethod]
        public void GithubIssue62_ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/62
            // A file encoded in a non-Unicode codepage (Windows-1251) that declares its
            // charset in a <meta> tag should stay readable after minification.
            // Arrange
            Encoding win1251 = Encoding.GetEncoding(1251);
            // "Привет мир" expressed as Unicode escapes so the test source encoding is irrelevant.
            string cyrillicText = "\u041F\u0440\u0438\u0432\u0435\u0442 \u043C\u0438\u0440";
            string cyrillicHtml =
                "<html>\r\n" +
                "  <head>\r\n" +
                "    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1251\" />\r\n" +
                "  </head>\r\n" +
                "  <body>\r\n" +
                "    <p>" + cyrillicText + "</p>\r\n" +
                "  </body>\r\n" +
                "</html>";

            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".html");
            File.WriteAllText(tempFile, cyrillicHtml, win1251);

            try
            {
                // Act
                Program.ProcessFile(noFeatures, tempFile);

                // Assert - read the minified file back using its original codepage and
                // confirm the Cyrillic text survived (it would be replaced with U+FFFD
                // '?' characters if the file was decoded/written as UTF-8).
                string result = File.ReadAllText(tempFile, win1251);
                StringAssert.Contains(result, cyrillicText);
                Assert.IsFalse(result.Contains("\uFFFD"), "Output contains Unicode replacement characters - encoding was corrupted.");
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
