using System.Linq;

namespace HtmlMinifier
{
    public class Features
    {
        /// <summary>
        /// Check the arguments passed in to determine if we should enable or disable any features.
        /// </summary>
        /// <param name="args">The arguments passed in.</param>
        public Features(string[] args)
        {
            if (args.Contains("ignorehtmlcomments"))
            {
                IgnoreHtmlComments = true;
            }

            if (args.Contains("ignorejscomments"))
            {
                IgnoreJsComments = true;
            }

            if (args.Contains("ignoreknockoutcomments"))
            {
                IgnoreKnockoutComments = true;
            }

            int maxLength = 0;

            // This is a check to see if the args contain an optional parameter for the max line length
            if (args != null && args.Length > 1)
            {
                // Try and parse the value sent through
                int.TryParse(args[1], out maxLength);
            }

            MaxLength = maxLength;
        }

        /// <summary>
        /// Should we ignore the JavaScript comments and not minify?
        /// </summary>
        public bool IgnoreJsComments { get; private set; }

        /// <summary>
        /// Should we ignore the html comments and not minify?
        /// </summary>
        public bool IgnoreHtmlComments { get; private set; }

        /// <summary>
        /// Should we ignore knockout comments?
        /// </summary>
        public bool IgnoreKnockoutComments { get; set; }

        /// <summary>
        /// Property for the max character count
        /// </summary>
        public int MaxLength { get; private set; }
    }
}
