using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlMinifier
{
    /// <summary>
    /// The html minification class.
    /// </summary>
    public class Program
    {
        private static long totalProcessed = 0;
        private static long totalSaved = 0;
        private static int totalFilesProcessed = 0;
        private static int totalFilesSkipped = 0;
        private static readonly object lockObject = new object();
        private static DateTime startTime;

        static void Main(string[] args)
        {
            try
            {
                // Check for help or version flags
                if (args.Length == 0)
                {
                    ConsoleReporter.ShowUsage();
                    Environment.Exit(1);
                    return;
                }

                if (args.Length == 1)
                {
                    var arg = args[0].ToLower();
                    if (arg == "--help" || arg == "-h" || arg == "/?" || arg == "-?")
                    {
                        ConsoleReporter.ShowHelp();
                        Environment.Exit(0);
                        return;
                    }
                    
                    if (arg == "--version" || arg == "-v")
                    {
                        ConsoleReporter.ShowVersion();
                        Environment.Exit(0);
                        return;
                    }
                }

                // Start timing
                startTime = DateTime.Now;

                // Show banner
                ConsoleReporter.ShowBanner();

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
                ConsoleReporter.ShowSummary(totalProcessed, totalSaved, totalFilesProcessed, 
                    totalFilesSkipped, errorCount, startTime);

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
                // Collect all HTML files from all directories
                var allHtmlFiles = new ConcurrentBag<string>();
                var directories = GetDirectories(folderPath);

                foreach (string folder in directories)
                {
                    try
                    {
                        string[] filePaths = Directory.GetFiles(folder);
                        foreach (var filePath in filePaths)
                        {
                            if (filePath.IsHtmlFile())
                            {
                                allHtmlFiles.Add(filePath);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        lock (lockObject)
                        {
                            Console.WriteLine($"Error: Access denied to directory {folder} - {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            Console.WriteLine($"Error: Failed to read directory {folder} - {ex.Message}");
                        }
                    }
                }

                // Process all files in parallel
                Parallel.ForEach(allHtmlFiles, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, filePath =>
                {
                    try
                    {
                        ProcessFile(features, filePath);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        lock (lockObject)
                        {
                            Console.WriteLine($"Error: Access denied to file {filePath} - {ex.Message}");
                        }
                    }
                    catch (IOException ex)
                    {
                        lock (lockObject)
                        {
                            Console.WriteLine($"Error: IO error processing file {filePath} - {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            Console.WriteLine($"Error: Failed to process file {filePath} - {ex.Message}");
                        }
                    }
                });
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

            lock (lockObject)
            {
                Console.WriteLine($"⚙  Processing: {Path.GetFileName(filePath)}");
            }

            try
            {
                // File size before minify
                var fileInfo = new FileInfo(filePath);
                var originalSize = fileInfo.Length;

                // Minify contents
                string minifiedContents = MinifyHtml(filePath, features);

                if (string.IsNullOrEmpty(minifiedContents))
                {
                    lock (lockObject)
                    {
                        Console.WriteLine($"⚠  Warning: Minification resulted in empty content for {filePath}. Skipping.");
                    }
                    Interlocked.Increment(ref totalFilesSkipped);
                    return;
                }

                // Write to the same file
                File.WriteAllText(filePath, minifiedContents, new UTF8Encoding(true));

                // File size after minify
                var newSize = new FileInfo(filePath).Length;

                // Thread-safe updates to totals
                Interlocked.Add(ref totalProcessed, originalSize);
                Interlocked.Add(ref totalSaved, newSize);
                Interlocked.Increment(ref totalFilesProcessed);

                var savedBytes = originalSize - newSize;
                var percentSaved = originalSize > 0 ? (savedBytes * 100.0 / originalSize) : 0;

                lock (lockObject)
                {
                    Console.WriteLine($"✓  {Path.GetFileName(filePath),-40} {ConsoleReporter.BytesToString(originalSize),8} → {ConsoleReporter.BytesToString(newSize),8} ({percentSaved:F1}% saved)");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                lock (lockObject)
                {
                    Console.WriteLine($"Error: Access denied to {filePath} - {ex.Message}");
                }
                throw;
            }
            catch (IOException ex)
            {
                lock (lockObject)
                {
                    Console.WriteLine($"Error: IO error with {filePath} - {ex.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                lock (lockObject)
                {
                    Console.WriteLine($"Error: Failed to minify {filePath} - {ex.Message}");
                }
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
    }
}
