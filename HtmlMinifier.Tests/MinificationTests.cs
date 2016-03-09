namespace HtmlMinifier.Tests
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class MinificationTests
    {
        readonly string _testDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Data");

        [Test]
        public void ReadHtml_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.StandardResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.Standard);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MinifyContents_WithComments_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.CommentsResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.Comments);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MinifyContents_WithModelList_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.ModelListResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.ModelList);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MinifyContents_WithLanguageSpecficCharacters_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.LanguageSpecificCharactersResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.LanguageSpecificCharacters);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GithubIssue10__ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/10                  
            // Arrange
            string expectedResult = DataHelpers.GithubIssue10Result;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.GithubIssue10);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GithubIssue13__ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/13                  
            string expectedResult = DataHelpers.GithubIssue13Result;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.GithubIssue13);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void SixtyFiveKCharacters__ShouldBreakToNextLine()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14                  
            List<string> args = new List<string> {"pathToFiles", "60000"};

            string expectedResult = DataHelpers.SixtyFiveThousandCharactersResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.SixtyFiveThousandCharacters);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, args.ToArray());

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void SixtyFiveKCharacters__WithoutArgs_ShouldMakeNoChange()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14                  
            List<string> args = new List<string> { "pathToFiles" };

            string expectedResult = DataHelpers.SixtyFiveThousandCharactersNoBreakResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.SixtyFiveThousandCharacters);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, args.ToArray());

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void RemoveJavaScriptComments_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.JavaScriptCommentsResult;

            // Act
            string removedComments = Program.RemoveJavaScriptComments(DataHelpers.JavaScriptComments);

            // Assert
            Assert.That(removedComments, Is.EqualTo(expectedResult));
        }

        [Test]
        public void RemoveMultipleJavaScriptComments_WithStandardText_ShouldReturnCorrectly()
        {
            // Arrange
            string expectedResult = DataHelpers.MultipleJavaScriptCommentsResult;

            // Act
            string removedComments = Program.RemoveJavaScriptComments(DataHelpers.MultipleJavaScriptComments);

            // Assert
            Assert.That(removedComments, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GithubIssue19Inherits__ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/19     
            string expectedResult = DataHelpers.GithubIssue19InheritsResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.GithubIssue19Inherits);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GithubIssue19Multiple__ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/19     
            string expectedResult = DataHelpers.GithubIssue19MultipleResult;

            // Act
            string minifiedHtml = Program.MinifyHtml(DataHelpers.GithubIssue19Multiple);

            minifiedHtml = Program.EnsureMaxLength(minifiedHtml, null);

            minifiedHtml = Program.ReArrangeDeclarations(minifiedHtml);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        #region Helpers

        public string ReadFileContents(string filePath)
        {
            string result;
            using (var reader = new StreamReader(filePath))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        #endregion
    }
}
