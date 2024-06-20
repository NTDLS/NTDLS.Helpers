using System.Text;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for dealing with text.
    /// </summary>
    public static class TextHelpers
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
    }
}
