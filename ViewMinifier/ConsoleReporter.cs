using System;
using System.Reflection;
using System.Text;

namespace HtmlMinifier
{
    /// <summary>
    /// Handles all console output and reporting for the HTML Minifier.
    /// </summary>
    public static class ConsoleReporter
    {
        /// <summary>
        /// Shows the welcome banner at the start of execution.
        /// </summary>
        public static void ShowBanner()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║         HTML Minifier v{0}.{1}.{2}             ║", version.Major, version.Minor, version.Build);
            Console.WriteLine("║    Fast parallel HTML minification tool       ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        /// <summary>
        /// Shows a comprehensive summary at the end of execution.
        /// </summary>
        /// <param name="totalProcessed">Total bytes before minification.</param>
        /// <param name="totalSaved">Total bytes after minification.</param>
        /// <param name="totalFilesProcessed">Number of files successfully processed.</param>
        /// <param name="totalFilesSkipped">Number of files skipped.</param>
        /// <param name="errorCount">Number of errors encountered.</param>
        /// <param name="startTime">Start time of processing.</param>
        public static void ShowSummary(long totalProcessed, long totalSaved, int totalFilesProcessed, 
            int totalFilesSkipped, int errorCount, DateTime startTime)
        {
            var endTime = DateTime.Now;
            var duration = (endTime - startTime).TotalSeconds;
            var savedBytes = totalProcessed - totalSaved;
            var percentSaved = totalProcessed > 0 ? (savedBytes * 100.0 / totalProcessed) : 0;
            var filesPerSecond = duration > 0 ? totalFilesProcessed / duration : 0;

            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║           MINIFICATION SUMMARY                 ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║ Files Processed:  {0,-28} ║", totalFilesProcessed.ToString("N0"));
            
            if (totalFilesSkipped > 0)
            {
                Console.WriteLine("║ Files Skipped:    {0,-28} ║", totalFilesSkipped.ToString("N0"));
            }
            
            Console.WriteLine("║ ─────────────────────────────────────────────  ║");
            Console.WriteLine("║ Size Before:      {0,-28} ║", BytesToString(totalProcessed));
            Console.WriteLine("║ Size After:       {0,-28} ║", BytesToString(totalSaved));
            Console.WriteLine("║ Total Saved:      {0,-28} ║", BytesToString(savedBytes));
            Console.WriteLine("║ Compression:      {0,-27}% ║", percentSaved.ToString("F1"));
            Console.WriteLine("║ ─────────────────────────────────────────────  ║");
            Console.WriteLine("║ Time Elapsed:     {0,-28} ║", FormatDuration(duration));
            Console.WriteLine("║ Throughput:       {0,-28} ║", $"{filesPerSecond:F1} files/sec");
            
            if (errorCount > 0)
            {
                Console.WriteLine("║ ─────────────────────────────────────────────  ║");
                Console.WriteLine("║ ⚠ Errors:        {0,-28} ║", errorCount.ToString("N0"));
            }
            
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            
            if (errorCount == 0)
            {
                Console.WriteLine("║              ✓ SUCCESS                         ║");
            }
            else
            {
                Console.WriteLine("║         ⚠ COMPLETED WITH ERRORS                ║");
            }
            
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        /// <summary>
        /// Shows brief usage information.
        /// </summary>
        public static void ShowUsage()
        {
            Console.WriteLine(GetUsageText());
        }

        /// <summary>
        /// Gets the usage text.
        /// </summary>
        /// <returns>The usage text.</returns>
        public static string GetUsageText()
        {
            return "Please provide folder path or file(s) to process\nUse --help for more information";
        }

        /// <summary>
        /// Shows detailed help information.
        /// </summary>
        public static void ShowHelp()
        {
            Console.WriteLine(GetHelpText());
        }

        /// <summary>
        /// Gets the help text.
        /// </summary>
        /// <returns>The help text.</returns>
        public static string GetHelpText()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var sb = new StringBuilder();
            
            sb.AppendLine($"HTML Minifier v{version.Major}.{version.Minor}.{version.Build}");
            sb.AppendLine("A fast and efficient tool to minify HTML, Razor views, and Web Forms views.");
            sb.AppendLine();
            sb.AppendLine("USAGE:");
            sb.AppendLine("  HtmlMinifier.exe <path> [options]");
            sb.AppendLine();
            sb.AppendLine("ARGUMENTS:");
            sb.AppendLine("  <path>                    File or folder path to process (supports multiple)");
            sb.AppendLine();
            sb.AppendLine("OPTIONS:");
            sb.AppendLine("  <number>                  Maximum line length (e.g., 60000)");
            sb.AppendLine("  ignorehtmlcomments        Preserve HTML comments (for Angular, etc.)");
            sb.AppendLine("  ignorejscomments          Preserve JavaScript comments");
            sb.AppendLine("  ignoreknockoutcomments    Preserve Knockout.js comments");
            sb.AppendLine("  --help, -h, /?            Show this help message");
            sb.AppendLine("  --version, -v             Show version information");
            sb.AppendLine();
            sb.AppendLine("EXAMPLES:");
            sb.AppendLine("  HtmlMinifier.exe \"C:\\MyProject\"");
            sb.AppendLine("  HtmlMinifier.exe \"C:\\MyProject\" 60000");
            sb.AppendLine("  HtmlMinifier.exe \"C:\\MyProject\" ignorehtmlcomments");
            sb.AppendLine("  HtmlMinifier.exe \"file1.html\" \"file2.html\"");
            sb.AppendLine();
            sb.AppendLine("SUPPORTED FILE TYPES:");
            sb.AppendLine("  .html, .htm, .cshtml, .vbhtml, .aspx, .ascx, .master, .inc");
            sb.AppendLine();
            sb.Append("For more information, visit: https://github.com/deanhume/html-minifier");
            
            return sb.ToString();
        }

        /// <summary>
        /// Shows version information.
        /// </summary>
        public static void ShowVersion()
        {
            Console.WriteLine(GetVersionText());
        }

        /// <summary>
        /// Gets the version text.
        /// </summary>
        /// <returns>The version text.</returns>
        public static string GetVersionText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var titleAttr = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var copyrightAttr = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var sb = new StringBuilder();
            
            sb.AppendLine($"{titleAttr?.Title ?? "HTML Minifier"} v{version.Major}.{version.Minor}.{version.Build}");
            sb.AppendLine(copyrightAttr?.Copyright ?? "Copyright Dean Hume");
            sb.Append("License: MIT");
            
            return sb.ToString();
        }

        /// <summary>
        /// Formats duration in a human-readable way.
        /// </summary>
        /// <param name="seconds">Duration in seconds.</param>
        /// <returns>Formatted duration string.</returns>
        private static string FormatDuration(double seconds)
        {
            if (seconds < 1)
                return $"{seconds * 1000:F0}ms";
            if (seconds < 60)
                return $"{seconds:F2}s";
            
            var minutes = (int)(seconds / 60);
            var secs = seconds % 60;
            return $"{minutes}m {secs:F0}s";
        }

        /// <summary>
        /// Converts bytes to a human readable string.
        /// </summary>
        /// <param name="byteCount">bytes</param>
        /// <returns>A human readable string</returns>
        public static string BytesToString(long byteCount)
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
