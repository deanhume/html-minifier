using System;
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
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Please provide folder path or file(s) to process");
                    Environment.Exit(1);
                    return;
                }

                // Determine which features to enable or disable
                var features = new Features(args);
                int errorCount = 0;

                foreach (var arg in args)
                {
                    // Skip feature flags
                    if (IsFeatureFlag(arg))
                        continue;

                    try
                    {
                        if (Directory.Exists(arg))
                        {
                            ProcessDirectory(features, arg);
                        }
                        else if (File.Exists(arg))
                        {
                            ProcessFile(features, arg);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Path not found - {arg}");
                            errorCount++;
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Error: Access denied to {arg} - {ex.Message}");
                        errorCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing {arg}: {ex.Message}");
                        errorCount++;
                    }
                }

                // Write the results
                Console.WriteLine("Minification Complete");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Total Processed: {0}", BytesToString(totalProcessed));
                Console.WriteLine("Total Minified: {0}", BytesToString(totalSaved));
                Console.WriteLine("Total Saved: {0}", BytesToString(totalProcessed - totalSaved));
                if (errorCount > 0)
                {
                    Console.WriteLine("Errors Encountered: {0}", errorCount);
                }
                Console.WriteLine("------------------------------------------");

                Environment.Exit(errorCount > 0 ? 1 : 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks if the argument is a feature flag.
        /// </summary>
        /// <param name="arg">The argument to check.</param>
        /// <returns>True if it's a feature flag, false otherwise.</returns>
        private static bool IsFeatureFlag(string arg)
        {
            var lowerArg = arg.ToLower();
            return lowerArg == "ignorehtmlcomments" ||
                   lowerArg == "ignorejscomments" ||
                   lowerArg == "ignoreknockoutcomments" ||
                   int.TryParse(arg, out _);
        }

        /// <summary>
        /// Minify all files in a given directory
        /// </summary>
        /// <param name="features">Features object</param>
        /// <param name="folderPath">The path to the folder</param>
        public static void ProcessDirectory(Features features, string folderPath)
        {
            try
            {
                // Loop through the files in the folder and look for any of the following extensions
                foreach (string folder in GetDirectories(folderPath))
                {
                    try
                    {
                        string[] filePaths = Directory.GetFiles(folder);
                        foreach (var filePath in filePaths)
                        {
                            if (filePath.IsHtmlFile())
                            {
                                try
                                {
                                    ProcessFile(features, filePath);
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    Console.WriteLine($"Error: Access denied to file {filePath} - {ex.Message}");
                                }
                                catch (IOException ex)
                                {
                                    Console.WriteLine($"Error: IO error processing file {filePath} - {ex.Message}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: Failed to process file {filePath} - {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Error: Access denied to directory {folder} - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: Failed to read directory {folder} - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to process directory {folderPath} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Minify a given file
        /// </summary>
        /// <param name="features">Features object</param>
        /// <param name="filePath">The path to the file</param>
        public static void ProcessFile(Features features, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            Console.WriteLine($"Beginning minification of: {filePath}");

            try
            {
                // File size before minify
                var fileInfo = new FileInfo(filePath);
                var originalSize = fileInfo.Length;
                totalProcessed += originalSize;

                // Minify contents
                string minifiedContents = MinifyHtml(filePath, features);

                if (string.IsNullOrEmpty(minifiedContents))
                {
                    Console.WriteLine($"Warning: Minification resulted in empty content for {filePath}. Skipping.");
                    return;
                }

                // Write to the same file
                File.WriteAllText(filePath, minifiedContents, new UTF8Encoding(true));

                // File size after minify
                var newSize = new FileInfo(filePath).Length;
                totalSaved += newSize;

                var savedBytes = originalSize - newSize;
                var percentSaved = originalSize > 0 ? (savedBytes * 100.0 / originalSize) : 0;

                Console.WriteLine($"Minified file: {filePath} (Saved: {BytesToString(savedBytes)}, {percentSaved:F1}%)");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access denied to {filePath} - {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: IO error with {filePath} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to minify {filePath} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the directories and subdirectories for a given path.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>A list of the directories.</returns>
        public static IEnumerable<string> GetDirectories(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be empty", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            try
            {
                // Get all subdirectories
                IEnumerable<string> directories = from subdirectory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories) select subdirectory;

                // Add the subdirectories
                IList<string> allDirectories = directories as IList<string> ?? directories.ToList();

                // Add the root folder
                allDirectories.Add(path);

                return allDirectories;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Warning: Access denied to some subdirectories in {path} - {ex.Message}");
                // Return at least the root directory
                return new List<string> { path };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to enumerate directories in {path} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Minifies the contents of the given view.
        /// </summary>
        /// <param name="filePath"> The file path. </param>
        /// <param name="features"> The features to apply. </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MinifyHtml(string filePath, Features features)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty", nameof(filePath));
            }

            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    return reader.MinifyHtmlCode(features);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found - {filePath}");
                throw new FileNotFoundException($"Cannot minify - file not found: {filePath}", ex);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: Cannot read file {filePath} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to minify HTML in {filePath} - {ex.Message}");
                throw;
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
