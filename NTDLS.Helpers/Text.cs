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
        /// Truncates a string at the first whitespace after a given length.
        /// </summary>
        public static string TruncateAtWord(string text, int minLength, bool addEllipsis = true)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= minLength)
                return text;

            int nextSpace = text.IndexOf(' ', minLength);
            if (nextSpace == -1)
                return text; // No whitespace found — return full text

            return text.Substring(0, nextSpace) + (addEllipsis ? "..." : string.Empty);
        }

        /// <summary>
        /// Returns a new string with the first occurrence of the given string replaced.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="search"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string input, string search, string replacement)
        {
            int pos = input.IndexOf(search);
            if (pos < 0)
            {
                return input; // Return the original string if the search string is not found
            }
            return input.Substring(0, pos) + replacement + input.Substring(pos + search.Length);
        }

        /// <summary>
        /// Replaces a range within a string with another string.
        /// </summary>
        public static string ReplaceRange(string original, int startIndex, int length, string replacement)
        {
            return original.Remove(startIndex, length).Insert(startIndex, replacement);
        }

        /// <summary>
        /// Inserts line-breaks after n-characters after given whitespace or punctuation are encountered.
        /// </summary>
        public static string SoftWrap(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var stringBuilder = new StringBuilder();
            var currentLineLength = 0;

            int i = 0;

            while (i < text.Length)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    i++;
                    continue;
                }

                stringBuilder.Append(text[i]);
                currentLineLength++;

                if (currentLineLength >= maxLineLength && (char.IsWhiteSpace(text[i]) || char.IsPunctuation(text[i])))
                {
                    stringBuilder.AppendLine();
                    currentLineLength = 0;

                    i++; //Skip "delimiter".

                    //Skip whitespace.
                    while (i < text.Length && char.IsWhiteSpace(text[i]))
                    {
                        i++;
                    }
                    continue;
                }

                i++;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Inserts line-breaks after n-characters after given characters are encountered.
        /// </summary>
        public static string SoftWrap(string text, int maxLineLength, char[] lineBreakOn)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var stringBuilder = new StringBuilder();
            var currentLineLength = 0;

            int i = 0;

            while (i < text.Length)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    i++;
                    continue;
                }

                stringBuilder.Append(text[i]);
                currentLineLength++;

                if (currentLineLength >= maxLineLength && lineBreakOn.Contains(text[i]))
                {
                    stringBuilder.AppendLine();
                    currentLineLength = 0;

                    i++; //Skip "delimiter".

                    //Skip whitespace.
                    while (i < text.Length && char.IsWhiteSpace(text[i]))
                    {
                        i++;
                    }
                    continue;
                }

                i++;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Adds a space to separate a set of camel-cased words.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SeparateCamelCase(string text)
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
        /// Splits a camel cased string into a list of tokens.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> SplitCamelCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new();
            }

            var matches = Regex.Matches(text, @"([A-Z][a-z]+|[a-z]+|[A-Z]+(?=[A-Z][a-z]|$))");
            var tokens = new List<string>();

            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }

            return tokens;
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

        /// <summary>
        /// Removes all whitespace from a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(string input)
            => new(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
    }
}
