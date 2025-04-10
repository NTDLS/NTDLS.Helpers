using System.Text.RegularExpressions;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for dealing with text (but specifically HTML).
    /// </summary>
    public static class Html
    {
        /// <summary>
        /// A class to represent a scope of HTML to remove.
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public class HtmlScope(string begin, string end)
        {
            /// <summary>
            /// The beginning of the scope to remove.
            /// </summary>
            public string Begin { get; set; } = begin;
            /// <summary>
            /// The end of the scope to remove.
            /// </summary>
            public string End { get; set; } = end;
        }

        /// <summary>
        /// Removes all traces of HTML tags from a given string.
        /// </summary>
        public static string StripHtml(string html)
        {
            html = (new Regex("<(.|\n)+?>")).Replace(html, " "); //Remove all text between < and >
            html = (new Regex("\\&(.|\n)+?\\;")).Replace(html, " "); //Remove all text between & and ;
            html = (new Regex("[^A-Za-z0-9]")).Replace(html, " "); // Remove all non-alphanumeric
            html = (new Regex(@"\s+")).Replace(html, " "); // compress all whitespace to one space.

            return html.Trim();
        }

        /// <summary>
        /// Removes all traces of HTML tags from a given string.
        /// </summary>
        public static string StripHtml(string html, HtmlScope[]? scopes = null, char[]? extraCharacters = null)
        {
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    if (string.IsNullOrEmpty(scope.Begin) || string.IsNullOrEmpty(scope.End))
                        continue;

                    string pattern = Regex.Escape(scope.Begin) + ".*?" + Regex.Escape(scope.End);
                    html = Regex.Replace(html, pattern, " ", RegexOptions.Singleline);
                }
            }

            if (extraCharacters != null)
            {
                foreach (var character in extraCharacters)
                {
                    html = html.Replace(character.ToString(), "");
                }
            }

            html = (new Regex("<(.|\n)+?>")).Replace(html, " "); //Remove all text between < and >
            html = (new Regex("\\&(.|\n)+?\\;")).Replace(html, " "); //Remove all text between & and ;
            html = (new Regex("[^A-Za-z0-9]")).Replace(html, " "); // Remove all non-alphanumeric.
            html = (new Regex(@"\s+")).Replace(html, " "); // compress all whitespace to one space.

            return html.Trim();
        }
    }
}
