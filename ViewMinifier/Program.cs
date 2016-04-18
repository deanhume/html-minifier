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
        public static Features _features = new Features();

        static void Main(string[] args)
        {
            string folderPath = GetFolderpath(args);

            IEnumerable<string> allDirectories = GetDirectories(folderPath);

            // Determine which features to enable or disable
            _features = FindValuesInArgs(args);

            // Loop through the files in the folder and look for any of the following extensions
            foreach (string folder in allDirectories)
            {
                string[] filePaths = Directory.GetFiles(folder);

                foreach (var filePath in filePaths)
                {
                    if (filePath.IsHtmlFile())
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
                contents = MinifyHtml(reader.ReadToEnd());

                // Ensure that the max length is less than 65K characters
                contents = EnsureMaxLength(contents, args);

                // Re-add the @model declaration
                contents = ReArrangeDeclarations(contents);
            }

            return contents;
        }

        /// <summary>
        /// Find any occurences of the particular Razor keywords 
        /// and add a new line or move to the top of the view.
        /// </summary>
        /// <param name="fileContents">The contents of the file</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReArrangeDeclarations(string fileContents)
        {   
            // A list of all the declarations
            Dictionary<string, bool> declarations = new Dictionary<string, bool>();
            declarations.Add("@model ", true);
            declarations.Add("@using ", false);
            declarations.Add("@inherits ", false);

            // Loop through the declarations
            foreach (var declaration in declarations)
            {
                fileContents = ReArrangeDeclaration(fileContents, declaration.Key, declaration.Value);
            }

            return fileContents;
        }

        /// <summary>
        /// Re-arranges the razor syntax on its own line.
        /// It seems to break the razor engine if this isnt on
        /// it's own line in certain cases.
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <param name="declaration">The declaration keywords that will cause a new line split.</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ReArrangeDeclaration(string fileContents, string declaration, bool bringToTop)
        {
            // Find possible multiple occurences in the file contents
            MatchCollection matches = Regex.Matches(fileContents, declaration);

            // Loop through the matches
            int alreadyMatched = 0;
            foreach (Match match in matches)
            {
                int position = declaration.Length;
                int declarationPosition = match.Index;

                // If we have more than one match, we need to keep the counter moving everytime we add a new line
                if (matches.Count > 1 && alreadyMatched > 0)
                {
                    // Cos we added one or more new line break \n\r
                    declarationPosition += (2 * alreadyMatched);
                }

                while (declarationPosition >= 0)
                {
                    // Move one forward
                    position += 1;
                    string substring = fileContents.Substring(declarationPosition, position);

                    // Check if it contains a whitespace at the end
                    if (substring.EndsWith(" ") || substring.EndsWith(">"))
                    {
                        if (bringToTop)
                        {
                            // First replace the occurence
                            fileContents = fileContents.Replace(substring, "");

                            // Next move it to the top on its own line
                            fileContents = substring + Environment.NewLine + fileContents;
                            break;
                        }
                        else
                        {
                            // Add a line break afterwards
                            fileContents = fileContents.Replace(substring, substring + Environment.NewLine);
                            alreadyMatched++;
                            break;
                        }
                    }
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
            // First, remove all JavaScript comments
            if (!_features.IgnoreJsComments)
            {
                htmlContents = RemoveJavaScriptComments(htmlContents);
            }

            // Minify the string
            htmlContents = Regex.Replace(htmlContents, @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/", "");

            // Replace line comments
            htmlContents = Regex.Replace(htmlContents, @"// (.*?)\r?\n", "", RegexOptions.Singleline);

            // Replace spaces between quotes
            htmlContents = Regex.Replace(htmlContents, @"\s+", " ");

            // Replace line breaks
            htmlContents = Regex.Replace(htmlContents, @"\s*\n\s*", "\n");

            // Replace spaces between brackets
            htmlContents = Regex.Replace(htmlContents, @"\s*\>\s*\<\s*", "><");

            // Replace comments
            if (!_features.IgnoreHtmlComments)
            {
                htmlContents = Regex.Replace(htmlContents, @"<!--(?!\[)(.*?)-->", "");
            }

            // single-line doctype must be preserved
            var firstEndBracketPosition = htmlContents.IndexOf(">", StringComparison.Ordinal);
            if (firstEndBracketPosition >= 0)
            {
                htmlContents = htmlContents.Remove(firstEndBracketPosition, 1);
                htmlContents = htmlContents.Insert(firstEndBracketPosition, ">");
            }

            return htmlContents.Trim();
        }

        /// <summary>
        /// Removes any JavaScript Comments in a script block
        /// </summary>
        /// <param name="javaScriptComments"></param>
        /// <returns>A string with all JS comments removed</returns>
        public static string RemoveJavaScriptComments(string javaScriptComments)
        {
            // Remove JavaScript comments
            Regex extractScripts = new Regex(@"<script[^>]*>[\s\S]*?</script>");

            // Loop through the script blocks
            foreach (Match match in extractScripts.Matches(javaScriptComments))
            {
                var scriptBlock = match.Value;

                javaScriptComments = javaScriptComments.Replace(scriptBlock, Regex.Replace(scriptBlock, @"[^:|""|']//(.*?)\r?\n", ""));

            }

            return javaScriptComments;
        }

        /// <summary>
        /// Ensure that the max character count is less than 65K.
        /// If so, break onto the next line.
        /// </summary>
        /// <param name="htmlContents">The minified HTML</param>
        /// <param name="args">An optional parameter for the max character count</param>
        /// <returns>A html string</returns>
        public static string EnsureMaxLength(string htmlContents, string[] args)
        {
            int maxLength = 60000;

            // This is a check to see if the args contain an optional parameter for the max line length
            if (args != null && args.Length > 1)
            {
                // Try and parse the value sent through
                if (!int.TryParse(args[1], out maxLength))
                {
                    maxLength = 60000;
                }

                int htmlLength = htmlContents.Length;
                int currentMaxLength = maxLength;
                int position;

                while (htmlLength > currentMaxLength)
                {
                    position = htmlContents.LastIndexOf("><", currentMaxLength);
                    htmlContents = htmlContents.Substring(0, position + 1) + "\r\n" + htmlContents.Substring(position + 1);
                    currentMaxLength += maxLength;
                }
            }

            return htmlContents;
        }

        /// <summary>
        /// Check the arguments passed in to determine if we should enable or disable any features.
        /// </summary>
        /// <param name="args">The arguments passed in.</param>
        /// <returns>A list of features to be enabled or disabled.</returns>
        public static Features FindValuesInArgs(string[] args)
        {
            _features = new Features();

            if (args.Contains("ignorehtmlcomments"))
            {
                _features.IgnoreHtmlComments = true;
            }

            if (args.Contains("ignorejscomments"))
            {
                _features.IgnoreJsComments = true;
            }

            return _features;
        }
    }
}
