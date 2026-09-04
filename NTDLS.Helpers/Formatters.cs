namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for formatting various data types.
    /// </summary>
    public class Formatters
    {
        private static readonly string[] _sizeFormats = { "{0} B", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(decimal size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(decimal size, int decimalPlaces)
            => FileSize((double)size, decimalPlaces);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(int size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(int size, int decimalPlaces)
            => FileSize((double)size, decimalPlaces);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(float size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(float size, int decimalPlaces)
            => FileSize((double)size, decimalPlaces);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(ulong size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(ulong size, int decimalPlaces)
            => FileSize((double)size, decimalPlaces);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(long size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(long size, int decimalPlaces)
            => FileSize((double)size, decimalPlaces);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(double size)
            => FileSize(size, size <= 1024 ? 0 : 2);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(double size, int decimalPlaces)
        {
            int i = 0;
            while (i < _sizeFormats.Length - 1 && size >= 1024)
            {
                size /= 1024.0;
                i++;
            }

            return string.Format(_sizeFormats[i], size.ToString($"N{decimalPlaces}"));
        }
    }
}
