namespace HtmlMinifier
{
    public static class StringExtension
    {
        /// <summary>
        /// Checks if a file extension matches 
        /// any given types for an ASP.net application
        /// </summary>
        /// <param name="value">The html file name.</param>
        /// <returns>A boolean if the file is an html file.</returns>
        public static bool IsHtmlFile(this string value)
        {
            var file = value.ToLower();

            return  file.EndsWith(".cshtml") ||
                    file.EndsWith(".vbhtml") ||
                    file.EndsWith(".aspx") ||
                    file.EndsWith(".html") ||
                    file.EndsWith(".htm") ||
                    file.EndsWith(".ascx") ||
                    file.EndsWith(".master") ||
                    file.EndsWith(".inc");
        }
    }
}
