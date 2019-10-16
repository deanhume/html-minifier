using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

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
    }
}
