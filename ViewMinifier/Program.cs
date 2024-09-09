﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HtmlMinifier
{
    /// <summary>
    /// The html minification class.
    /// </summary>
    public class Program
    {
        public static long totalProcessed = 0;
        public static long totalSaved = 0;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide folder path or file(s) to process");
            }
            else
            {
                // Determine which features to enable or disable
                var features = new Features(args);
                foreach (var arg in args)
                {
                    if (Directory.Exists(arg))
                    {
                        ProcessDirectory(features, arg);
                    }
                    else if (File.Exists(arg))
                    {
                        ProcessFile(features, arg);
                    }
                }

                // Write the results
                Console.WriteLine("Minification Complete");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Total Processed: {0}", BytesToString(totalProcessed));
                Console.WriteLine("Total Minified: {0}", BytesToString(totalSaved));
                Console.WriteLine("Total Saved: {0}", BytesToString(totalProcessed - totalSaved));
                Console.WriteLine("------------------------------------------");

            }
        }

        /// <summary>
        /// Minify all files in a given file
        /// </summary>
        /// <param name="features">Features object</param>
        /// <param name="folderPath">The path to the folder</param>
        public static void ProcessDirectory(Features features, string folderPath)
        {
            // Loop through the files in the folder and look for any of the following extensions
            foreach (string folder in GetDirectories(folderPath))
            {
                string[] filePaths = Directory.GetFiles(folder);
                foreach (var filePath in filePaths)
                {
                    if (filePath.IsHtmlFile())
                        ProcessFile(features, filePath);
                }
            }
        }

        /// <summary>
        /// Minify a given file
        /// </summary>
        /// <param name="features">Features object</param>
        /// <param name="filePath">The path to the file</param>
        public static void ProcessFile(Features features, string filePath)
        {
            Console.WriteLine("Beginning Minification");

            // File size before minify
            totalProcessed += new FileInfo(filePath).Length;

            // Minify contents
            string minifiedContents = MinifyHtml(filePath, features);

            // Write to the same file
            File.WriteAllText(filePath, minifiedContents, new UTF8Encoding(true));

            // File size after minify
            totalSaved += new FileInfo(filePath).Length;

            Console.WriteLine("Minified file : " + filePath);
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

        /// <summary>
        /// Converts bytes to a human readable string.
        /// </summary>
        /// <param name="byteCount">bytes</param>
        /// <returns>A human readable string</returns>
        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

            if (byteCount == 0)
                return "0" + suf[0];

            long bytes = Math.Abs(byteCount);

            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
