using System.Text;
using System.Text.RegularExpressions;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for dealing with text.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Inserts a line-break every n-characters where a line break exists.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        public static string InsertLineBreaks(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var words = text.Split(' ');
            var stringBuilder = new StringBuilder();
            var currentLineLength = 0;

            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > maxLineLength)
                {
                    stringBuilder.AppendLine();
                    currentLineLength = 0;
                }

                if (currentLineLength > 0)
                {
                    stringBuilder.Append(' ');
                    currentLineLength++;
                }

                stringBuilder.Append(word);
                currentLineLength += word.Length;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Adds a space to seperate a set of camel-cased words.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SeperateCamelCase(string text)
        {
            return Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                text,
                                @"([a-z])([A-Z])", // Lowercase followed by uppercase
                                "$1 $2"
                            ),
                            @"([A-Z])([A-Z][a-z])", // Uppercase followed by uppercase and lowercase
                            "$1 $2"
                        ),
                        @"\s+",
                        " "
                    );
        }

        /// <summary>
        /// Replaces the first occurrence of a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirstOccurrence(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
