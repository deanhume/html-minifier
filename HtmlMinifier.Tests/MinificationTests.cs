namespace HtmlMinifier.Tests
{
    using System;
    using System.IO;

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
