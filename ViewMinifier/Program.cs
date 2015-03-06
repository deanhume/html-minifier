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

            IEnumerable<string> allDirectories = GetDirectories(folderPath);

            // Loop through the files in the folder and look for *.cshtml & *.aspx
            foreach (string folder in allDirectories)
            {
                string[] filePaths = Directory.GetFiles(folder);

                foreach (var filePath in filePaths)
                {
                    if (filePath.Contains(".cshtml") || filePath.Contains(".vbhtml") || filePath.Contains(".aspx") || filePath.Contains(".html") || filePath.Contains(".htm") || filePath.Contains(".ascx"))
                    {
                        // Minify contents
                        string minifiedContents = ReadHtml(filePath, args);

                        // Write to the same file
                        File.WriteAllText(filePath, minifiedContents);

                        Console.WriteLine("Minified file : " + filePath);
                    }
                }
            }

            Console.WriteLine("Minification Complete");
        }

        /// <summary>
        /// Gets the directories and subdirectories for a given path.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>A list of the directories.</returns>
        public static IEnumerable<string> GetDirectories(string path)
        {
            // Get all subdirectories
            IEnumerable<string> directories = from subdirectory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories) select subdirectory;

            // Add the subdirectories
            IList<string> allDirectories = directories as IList<string> ?? directories.ToList();

            // Add the root folder
            allDirectories.Add(path);

            return allDirectories;
        }

        /// <summary>
        /// Get the folder path
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A string with the folder path</returns>
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
        public static string ReadHtml(string filePath, string[] args = null)
        {
            // Read in the file contents
            string contents;
            using (var reader = new StreamReader(filePath))
            {
                // Minify the contents
                contents = EnsureMaxLength(MinifyHtml(reader.ReadToEnd()), args);

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

        public static string EnsureMaxLength(string htmlContents, string[] args)
        {
            int iMaxLength = 60000;

            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out iMaxLength)) iMaxLength = 60000;
            }

            htmlContents = htmlContents.Trim();

            int iLen = htmlContents.Length;
            int iCurMaxLength = iMaxLength;
            int iPos;

            while (iLen > iCurMaxLength)
            {
                iPos = htmlContents.LastIndexOf("><", iCurMaxLength);
                htmlContents = htmlContents.Substring(0, iPos + 1) + "\n" + htmlContents.Substring(iPos + 1);
                iCurMaxLength += iMaxLength;
            }

            return htmlContents;
        }
    }
}
