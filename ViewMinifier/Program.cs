namespace HtmlMinifier
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    class Program
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
                        string minifiedContents = MinifyContents(filePath);

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
        private static string MinifyContents(string filePath)
        {
            // Read in the file contents
            string readToEnd;
            using (var reader = new StreamReader(filePath))
            {
                readToEnd = reader.ReadToEnd();

                // Replace spaces between quotes
                readToEnd = Regex.Replace(readToEnd, @"\s+", " ");
                
                // Replace line breaks
                readToEnd = Regex.Replace(readToEnd, @"\s*\n\s*", "\n");

                // Replace spaces between brackets
                readToEnd = Regex.Replace(readToEnd, @"\s*\>\s*\<\s*", "><");
                
                // Replace comments
                readToEnd = Regex.Replace(readToEnd, @"<!--(.*?)-->", "");

                // single-line doctype must be preserved 
                var firstEndBracketPosition = readToEnd.IndexOf(">", System.StringComparison.Ordinal);
                if (firstEndBracketPosition >= 0)
                {
                    readToEnd = readToEnd.Remove(firstEndBracketPosition, 1);
                    readToEnd = readToEnd.Insert(firstEndBracketPosition, ">");
                }
            }

            return readToEnd;
        }
    }
}
