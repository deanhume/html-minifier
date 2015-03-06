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
            string filePath = Path.Combine(_testDataFolder, "standard.txt");

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "standardresult.txt"));

            // Act
            string minifiedHtml = Program.ReadHtml(filePath);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MinifyContents_WithComments_ShouldReturnCorrectly()
        {
            // Arrange
            string filePath = Path.Combine(_testDataFolder, "comments.txt");

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "commentsresult.txt"));

            // Act
            string minifiedHtml = Program.MinifyHtml(this.ReadFileContents(filePath));

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MinifyContents_WithModelList_ShouldReturnCorrectly()
        {
            // Arrange
            string filePath = Path.Combine(_testDataFolder, "ModelList.txt");

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "ModelListResult.txt"));

            // Act
            string minifiedHtml = Program.ReadHtml(filePath);

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetDirectories_WithFolderPath_ReturnsRootAndSubdirectories()
        {
            // Arrange
 
            // Act
            IEnumerable<string> rootAndSubdirectories = Program.GetDirectories(_testDataFolder);

            // Assert
            Assert.That(rootAndSubdirectories.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetDirectories_WithFolderPath_ReturnsRoot()
        {
            // Arrange
            string rootFolderPath = Path.Combine(_testDataFolder, @"Subdirectory");

            // Act
            IEnumerable<string> rootAndSubdirectories = Program.GetDirectories(rootFolderPath);

            // Assert
            Assert.That(rootAndSubdirectories.Count(), Is.EqualTo(1));
        }

        [Test]
        public void MinifyContents_WithLanguageSpecficCharacters_ShouldReturnCorrectly()
        {
            string filePath = Path.Combine(_testDataFolder, "LanguageSpecificCharacters.txt");

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "LanguageSpecificCharactersResult.txt"));

            // Act
            string minifiedHtml = Program.MinifyHtml(this.ReadFileContents(filePath));

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GithubIssue10__ShouldReturnCorrectly()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/10                  
            string filePath = Path.Combine(_testDataFolder, "GithubIssue10.txt");

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "GithubIssue10Result.txt"));

            // Act
            string minifiedHtml = Program.MinifyHtml(this.ReadFileContents(filePath));

            // Assert
            Assert.That(minifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void SixtyFiveKCharacters__ShouldBreakToNextLine()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14                  
            string filePath = Path.Combine(_testDataFolder, "65K.txt");
            List<string> args = new List<string> {"pathToFiles", "60000"};

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "65KResult.txt"));

            // Act
            string minifiedHtml = Program.MinifyHtml(this.ReadFileContents(filePath));

            string newMinifiedHtml = Program.EnsureMaxLength(minifiedHtml, args.ToArray());

            // Assert
            Assert.That(newMinifiedHtml, Is.EqualTo(expectedResult));
        }

        [Test]
        public void SixtyFiveKCharacters__WithoutArgs_ShouldMakeNoChange()
        {
            // A fix for a Github issue - https://github.com/deanhume/html-minifier/issues/14                  
            string filePath = Path.Combine(_testDataFolder, "Standard.txt");
            List<string> args = new List<string> { "pathToFiles" };

            string expectedResult = ReadFileContents(Path.Combine(_testDataFolder, "StandardResult.txt"));

            // Act
            string minifiedHtml = Program.MinifyHtml(this.ReadFileContents(filePath));

            string newMinifiedHtml = Program.EnsureMaxLength(minifiedHtml, args.ToArray());

            string reArranged = Program.ReArrangeModelDeclaration(newMinifiedHtml);

            // Assert
            Assert.That(reArranged, Is.EqualTo(expectedResult));
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
