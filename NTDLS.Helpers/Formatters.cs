namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for formatting various data types.
    /// </summary>
    public class Formatters
    {
        /// <summary>
        /// Formats a file size to b,kb,mb, etc.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FileSize(long size)
        {
            double s = size;

            string[] format = new string[] { "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };

            int i = 0;
            while (i < format.Length && s >= 1024)
            {
                s = (int)(100 * s / 1024) / 100.0;
                i++;
            }

            return string.Format(format[i], s);
        }
    }
}
