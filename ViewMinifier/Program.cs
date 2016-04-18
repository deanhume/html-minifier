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

            // Determine which features to enable or disable
            var features = new Features(args);

            // Loop through the files in the folder and look for any of the following extensions
            foreach (string folder in allDirectories)
            {
                string[] filePaths = Directory.GetFiles(folder);

                foreach (var filePath in filePaths)
                {
                    if (filePath.IsHtmlFile())
                    {
                        // Minify contents
                        string minifiedContents = MinifyHtml(filePath, features);

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
        /// <param name="filePath"> The file path. </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MinifyHtml(string filePath, Features features)
        {
            using (var reader = new StreamReader(filePath))
            {
                return reader.MinifyHtmlCode(features);                
            }
        }
    }
}
