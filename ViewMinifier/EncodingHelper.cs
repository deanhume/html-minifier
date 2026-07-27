using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlMinifier
{
    /// <summary>
    /// Helpers for detecting the encoding of a file so it can be preserved when the
    /// file is rewritten. See GitHub issue #62 - files encoded in a non-Unicode
    /// codepage (e.g. Windows-1251) would otherwise be decoded/written as UTF-8 and
    /// corrupted.
    /// </summary>
    public static class EncodingHelper
    {
        // Matches a charset declaration in an HTML5 <meta charset="..."> tag or a
        // legacy <meta http-equiv="Content-Type" content="text/html; charset=..."> tag.
        private static readonly Regex CharsetRegex = new Regex(
            "charset\\s*=\\s*[\"']?\\s*([a-zA-Z0-9_\\-]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Detects the encoding of a file so it can be preserved when the file is
        /// rewritten. A byte order mark takes precedence; otherwise a charset declared
        /// in a &lt;meta&gt; tag is used; finally it falls back to UTF-8 (no BOM).
        /// </summary>
        /// <param name="filePath">The file to inspect.</param>
        /// <returns>The detected <see cref="Encoding"/>. Encodings returned for BOM-prefixed
        /// files emit their BOM when written; all others emit no preamble.</returns>
        public static Encoding DetectEncoding(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty", nameof(filePath));
            }

            byte[] header;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int length = (int)Math.Min(stream.Length, 4096);
                header = new byte[length];
                int read = 0;
                while (read < length)
                {
                    int chunk = stream.Read(header, read, length - read);
                    if (chunk == 0)
                    {
                        break;
                    }
                    read += chunk;
                }
            }

            // 1. Byte order marks (checked longest-first to avoid ambiguity).
            if (header.Length >= 4 && header[0] == 0xFF && header[1] == 0xFE && header[2] == 0x00 && header[3] == 0x00)
            {
                return new UTF32Encoding(bigEndian: false, byteOrderMark: true);
            }
            if (header.Length >= 4 && header[0] == 0x00 && header[1] == 0x00 && header[2] == 0xFE && header[3] == 0xFF)
            {
                return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            }
            if (header.Length >= 3 && header[0] == 0xEF && header[1] == 0xBB && header[2] == 0xBF)
            {
                return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            }
            if (header.Length >= 2 && header[0] == 0xFF && header[1] == 0xFE)
            {
                return new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
            }
            if (header.Length >= 2 && header[0] == 0xFE && header[1] == 0xFF)
            {
                return new UnicodeEncoding(bigEndian: true, byteOrderMark: true);
            }

            // 2. Charset declared in a <meta> tag. The declaration itself is ASCII,
            // so decoding the header as ASCII is enough to find it.
            string headerText = Encoding.ASCII.GetString(header);
            Match match = CharsetRegex.Match(headerText);
            if (match.Success)
            {
                try
                {
                    Encoding declared = Encoding.GetEncoding(match.Groups[1].Value);

                    // Do not add a BOM the source file did not have.
                    if (declared is UTF8Encoding)
                    {
                        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                    }

                    return declared;
                }
                catch (ArgumentException)
                {
                    // Unknown or unsupported charset name - fall back to the default below.
                }
            }

            // 3. Default: UTF-8 without a BOM.
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        }
    }
}
