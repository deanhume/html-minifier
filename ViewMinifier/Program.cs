namespace HtmlMinifier
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The html minification class.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            string folderPath = GetFolderpath(args);

            IEnumerable<string> allDirectories = GetSubdirectoriesContainingOnlyFiles(folderPath);

            // Loop through the files in the folder and look for *.cshtml & *.aspx
            foreach (string folder in allDirectories)
            {
                string[] filePaths = Directory.GetFiles(folder);

                foreach (var filePath in filePaths)
                {
                    if (filePath.Contains(".cshtml") || filePath.Contains(".vbhtml") || filePath.Contains(".aspx") || filePath.Contains(".html") || filePath.Contains(".htm"))
                    {
                        // Minify contents
                        string minifiedContents = ReadHtml(filePath);

                        // Write to the same file
                        File.WriteAllText(filePath, minifiedContents);

                        Console.WriteLine("Minified file : " + filePath);
                    }
                }
            }

            Console.WriteLine("Minification Complete");
        }

        static IEnumerable<string> GetSubdirectoriesContainingOnlyFiles(string path)
        {
            // Get all subdirectories
            IEnumerable<string> directories = from subdirectory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories) select subdirectory;

            // If there are no subdirectories, use the root folder
            IList<string> allDirectories = directories as IList<string> ?? directories.ToList();
            if (!allDirectories.Any())
            {
                allDirectories.Add(path);
            }

            return allDirectories;
        }

        private static string GetFolderpath(string[] args)
        {
            // Check that the folder path is provided
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the folder path");

                Environment.Exit(0);
            }

            // Return the folder path
            return args[0];
        }

        /// <summary>
        /// Minifies the contents of the given view.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadHtml(string filePath)
        {
            // Read in the file contents
            string contents;
            using (var reader = new StreamReader(filePath))
            {
                // Minify the contents
                contents = MinifyHtml(reader.ReadToEnd());

                // Re-add the @model declaration
                contents = ReArrangeModelDeclaration(contents);
            }

            return contents;
        }

        /// <summary>
        /// Re-arranges the razor syntax with the @model declaration on its
        /// own line. It seems to break the razor engine if this isnt on
        /// it's own line.
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReArrangeModelDeclaration(string fileContents)
        {
            int modelPosition = fileContents.IndexOf("@model ");

            int position = 7;
            while (modelPosition >= 0)
            {
                // move one forward
                position += 1;
                string substring = fileContents.Substring(modelPosition, position);

                // check if it contains a whitespace at the end
                if (substring.EndsWith(" ") || substring.EndsWith(">"))
                {
                    // first replace the occurence
                    fileContents = fileContents.Replace(substring, "");

                    // Next move it to the top on its own line
                    fileContents = substring + Environment.NewLine + fileContents;

                    return fileContents;
                }
            }

            return fileContents;
        }

        /// <summary>
        /// Minifies the given HTML string.
        /// </summary>
        /// <param name="htmlContents">
        /// The html to minify.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MinifyHtml(string htmlContents)
        {
            // Replace line comments
            htmlContents = Regex.Replace(htmlContents, @"// (.*?)\r?\n", "", RegexOptions.Singleline);

            // Replace spaces between quotes
            htmlContents = Regex.Replace(htmlContents, @"\s+", " ");

            // Replace line breaks
            htmlContents = Regex.Replace(htmlContents, @"\s*\n\s*", "\n");

            // Replace spaces between brackets
            htmlContents = Regex.Replace(htmlContents, @"\s*\>\s*\<\s*", "><");

            // Replace comments
            htmlContents = Regex.Replace(htmlContents, @"<!--(?!\[)(.*?)-->", "");

            // single-line doctype must be preserved 
            var firstEndBracketPosition = htmlContents.IndexOf(">", StringComparison.Ordinal);
            if (firstEndBracketPosition >= 0)
            {
                htmlContents = htmlContents.Remove(firstEndBracketPosition, 1);
                htmlContents = htmlContents.Insert(firstEndBracketPosition, ">");
            }
            return htmlContents.Trim();
        }
    }
}
