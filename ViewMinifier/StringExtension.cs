namespace HtmlMinifier
{
    public static class StringExtension
    {
        public static bool IsHtmlFile(this string value)
        {
            var file = value.ToLower();
            return  file.EndsWith(".cshtml") ||
                    file.EndsWith(".vbhtml") ||
                    file.EndsWith(".aspx") ||
                    file.EndsWith(".html") ||
                    file.EndsWith(".htm") ||
                    file.EndsWith(".ascx") ||
                    file.EndsWith(".master");
        }
    }
}
