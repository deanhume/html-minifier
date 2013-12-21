namespace HtmlMinifier
{
    using WebMarkupMin.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using WebMarkupMin.Core.Settings;

    class Program
    {
        // Accepts a pathspec either folder/*.cshtml or file.cshtml and saves files with .min. instead of overwriting the original
        // The following parameters are name=value pairs to override the default minifier settings
        static void Main(string[] args)
        {
            
            var filespec = GetFilespec(args);
            var filePattern = Path.GetFileName(filespec);
            var folderName = Path.GetDirectoryName(filespec);
            if (folderName == String.Empty)
                folderName = ".";

            IEnumerable<string> allDirectories = GetSubdirectoriesContainingOnlyFiles(folderName, filePattern);
            var settings = GetMinifierArgs(args);
            var paras = GetParams(args);
            var minifier = GetMinifier(settings);
            int warnings = 0, errors = 0;

            // Loop through the files in the folder and look for *.cshtml & *.aspx
            foreach (string folder in allDirectories)
            {
                string[] filePaths = Directory.GetFiles(folder, filePattern, paras.Contains("-r") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foreach (var filePath in filePaths)
                {
                    var fp = filePath.ToLower();
                    var fileExtensions = new string[] {
                        ".cshtml", "vbhtml", ".aspx", ".html", ".htm"
                    };
                    if (!fp.Contains(".min.") && (fileExtensions.Any(fe => Path.GetExtension(fe)== Path.GetExtension(fp))))
                    {
                        // Minify contents
                        var fileText = File.ReadAllText(fp);
                        var minifiedContents = minifier.Minify(fileText, true);

                        // Write to output filename
                        var outputPath = Path.Combine(Path.GetDirectoryName(fp), Path.GetFileNameWithoutExtension(fp) + ".min" + Path.GetExtension(fp));
                        if (String.IsNullOrWhiteSpace(minifiedContents.MinifiedContent))
                            File.Delete(outputPath);
                        else
                            File.WriteAllText(outputPath, minifiedContents.MinifiedContent);

                        Console.WriteLine(String.Format("{0} ({3} bytes) => {1} ({4} bytes {2}%)", fp, outputPath, minifiedContents.Statistics.CompressionRatio,
                            minifiedContents.Statistics.OriginalSize, minifiedContents.Statistics.MinifiedSize));
                        OutputErrors(minifiedContents.Errors, ConsoleColor.Red);
                        OutputErrors(minifiedContents.Warnings, ConsoleColor.DarkYellow);
                        warnings += minifiedContents.Warnings.Count();
                        errors += minifiedContents.Errors.Count();
                    }
                }
            }
            Console.WriteLine(String.Format("\n{0} Warnings, {1} Errors", warnings, errors));
            Environment.Exit(errors);
        }

        static void OutputErrors(IList<WebMarkupMin.Core.Minifiers.MinificationErrorInfo> ei, ConsoleColor c)
        {
            foreach(WebMarkupMin.Core.Minifiers.MinificationErrorInfo e in ei)
            {
                using (new ConsoleColour(c))
                    Console.Error.WriteLine(String.Format("[{0}] : {2}({3}) {1}", e.Category, e.Message, e.LineNumber, e.ColumnNumber));
                using (new ConsoleColour(ConsoleColor.Gray))
                    Console.Error.WriteLine(e.SourceFragment);
            }
        }

        static IEnumerable<string> GetSubdirectoriesContainingOnlyFiles(string path, string filespec)
        {
            // Get all subdirectories
            IEnumerable<string> directories = from subdirectory in Directory.GetDirectories(path, filespec, SearchOption.AllDirectories) select subdirectory;

            // If there are no subdirectories, use the root folder
            IList<string> allDirectories = directories as IList<string> ?? directories.ToList();
            if (!allDirectories.Any())
            {
                allDirectories.Add(path);
            }

            return allDirectories;
        }

        private static string GetFilespec(string[] args)
        {
            // Check that the folder path is provided
            if (args.Length == 0)
            {
                Console.WriteLine("\nMissing file specification. Usage;\n\n\t" +
                                    System.Diagnostics.Process.GetCurrentProcess().ProcessName +
                                    " <filespec> [name=value,...]\n\n" +
                                    "wherefilespec can be folder/* or folder/file.cshtml and where name and value are from HtmlMinifier settings\n\n ");
                Console.WriteLine(String.Join("\n", typeof(HtmlMinificationSettings).GetProperties().Select(prop => String.Format("{0} is <{1}>", prop.Name, prop.PropertyType))));
                Environment.Exit(0);
            }

            // Return the folder path
            return args[0];
        }

        // TODO: could combine this into one loop and return both params and arguments
        private static IList<string> GetParams(string[] args)
        {
            var list = new List<string>();
            for (var i = 1; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "-r":
                            list.Add(arg);
                        break;

                        default:
                            throw new ArgumentException("Unknown parameter : " + arg);
                    }
                }
            }

            return list;
        }

        // TODO: could combine this into one loop and return both params and arguments
        private static IDictionary<string, string> GetMinifierArgs(string[] args)
        {
            var dict = new Dictionary<string, string>();
            for (var i = 1; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-") == false)
                {
                    var parts = arg.Split('=');
                    if (parts.Length != 2)
                        throw new ArgumentException("Unknown argument : " + arg);
                    dict.Add(parts[0], parts[1]);
                }
            }

            return dict;
        }

        /// <summary>
        /// Minifies the contents of the given view.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        private static WebMarkupMin.Core.Minifiers.HtmlMinifier GetMinifier(IDictionary<string,string> minifierProperties = null)
        {
            // Defaults. Could get these from the app.config
            var minifierSettings = new HtmlMinificationSettings()
            { 
                WhitespaceMinificationMode = WhitespaceMinificationMode.Medium,
                RemoveHtmlComments = true,
                RemoveHtmlCommentsFromScriptsAndStyles = true,
                RemoveCdataSectionsFromScriptsAndStyles = true,
                UseShortDoctype = true ,
                UseMetaCharsetTag = true,
                EmptyTagRenderMode = HtmlEmptyTagRenderMode.Slash,
                RemoveOptionalEndTags = false,
                RemoveTagsWithoutContent = false,
                CollapseBooleanAttributes = true,
                RemoveEmptyAttributes = false ,
                AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes,
                RemoveRedundantAttributes = true,
                RemoveJsTypeAttributes = true ,
                RemoveCssTypeAttributes = true,
                RemoveHttpProtocolFromAttributes = false,
                RemoveHttpsProtocolFromAttributes = false,
                RemoveJsProtocolFromAttributes = true,
                MinifyEmbeddedCssCode = true,
                MinifyInlineCssCode = true,
                MinifyEmbeddedJsCode = true, 
                MinifyInlineJsCode = true,
            };

            // Loop through property bag setting values passed in on command line
            foreach (var setting in minifierProperties)
            {
                var prop = minifierSettings.GetType().GetProperty(setting.Key);
                if (prop != null)
                {
                    try
                    {
                        if (prop.PropertyType.IsEnum)
                            prop.SetValue(minifierSettings, Enum.Parse(prop.PropertyType, setting.Value));
                        else
                            prop.SetValue(minifierSettings, bool.Parse(setting.Value));
                    }
                    catch (FormatException fe)
                    {
                        throw new FormatException(String.Format("Invalid property value '{0}' for '{1}'", setting.Value, prop.Name), fe);
                    }
                }
                else
                    throw new ArgumentException("Unknown property '" + setting.Key + "' specified.");
            }

            return new WebMarkupMin.Core.Minifiers.HtmlMinifier(minifierSettings,
                new WebMarkupMin.Core.Minifiers.KristensenCssMinifier(),
                new WebMarkupMin.Core.Minifiers.CrockfordJsMinifier());
        }
    }
}
