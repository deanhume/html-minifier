using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlMinifier
{
    public static class StreamReaderExtension
    {
        /// <summary>
        /// Minify the HTML code
        /// </summary>
        /// <param name="reader">The StreamReader.</param>
        /// <param name="features">Any features to enable / disable.</param>
        /// <returns>The minified HTML code.</returns>
        public static string MinifyHtmlCode(this StreamReader reader, Features features)
        {
            return MinifyHtmlCode(reader.ReadToEnd(), features);
        }

        /// <summary>
        /// Minifies the HTML code
        /// </summary>
        /// <param name="htmlCode">The HTML as a string</param>
        /// <param name="features">Any features to enable / disable.</param>
        /// <returns>The minified HTML code.</returns>
        public static string MinifyHtmlCode(string htmlCode, Features features)
        {
            string contents;

            // Minify the contents
            contents = MinifyHtml(htmlCode, features);

            // Ensure that the max length is less than 65K characters
            contents = EnsureMaxLength(contents, features);

            // Re-add the @model declaration
            contents = ReArrangeDeclarations(contents);

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
                    if (position > fileContents.Length) break;
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
        /// <param name="htmlContents"> The html to minify.</param>
        /// <param name="features">The features</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MinifyHtml(string htmlContents, Features features)
        {
            // First, remove all JavaScript comments
            if (!features.IgnoreJsComments)
            {
                htmlContents = RemoveJavaScriptComments(htmlContents);
            }

            // Remove special keys
            htmlContents = htmlContents.Replace("/*", "{{{SLASH_STAR}}}");

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
            if (!features.IgnoreHtmlComments)
            {
                if (features.IgnoreKnockoutComments)
                {
                    htmlContents = Regex.Replace(htmlContents, @"<!--(?!(\[|\s*#include))(?!ko .*)(?!\/ko)(.*?)-->", "");
                }
                else
                {
                    htmlContents = Regex.Replace(htmlContents, @"<!--(?!(\[|\s*#include))(.*?)-->", "");
                }
            }

            // single-line doctype must be preserved
            var firstEndBracketPosition = htmlContents.IndexOf(">", StringComparison.Ordinal);
            if (firstEndBracketPosition >= 0)
            {
                htmlContents = htmlContents.Remove(firstEndBracketPosition, 1);
                htmlContents = htmlContents.Insert(firstEndBracketPosition, ">");
            }

            // Put back special keys
            htmlContents = htmlContents.Replace("{{{SLASH_STAR}}}", "/*");
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
        /// <param name="features">The features</param>
        /// <returns>A html string</returns>
        public static string EnsureMaxLength(string htmlContents, Features features)
        {
            if (features.MaxLength > 0)
            {
                int htmlLength = htmlContents.Length;
                int currentMaxLength = features.MaxLength;
                int position;

                while (htmlLength > currentMaxLength)
                {
                    position = htmlContents.LastIndexOf("><", currentMaxLength);
                    htmlContents = htmlContents.Substring(0, position + 1) + "\r\n" + htmlContents.Substring(position + 1);
                    currentMaxLength += features.MaxLength;
                }
            }
            return htmlContents;
        }
    }
}
