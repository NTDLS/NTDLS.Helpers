namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for handling exceptions
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        /// Delegate for ignoring exceptions.
        /// </summary>
        public delegate void TryAndIgnoreProc();

        /// <summary>
        /// Delegate for ignoring exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T TryAndIgnoreProc<T>();

        /// <summary>
        /// Recursively gets the exception at the bottom of an exception -> InnerException stack.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception GetRootException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetRootException(ex.InnerException);
            }
            return ex;
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions.
        /// </summary>
        /// <param name="func"></param>
        public static void Ignore(TryAndIgnoreProc func)
        {
            try { func(); } catch { }
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T? Ignore<T>(TryAndIgnoreProc<T> func)
        {
            try { return func(); } catch { }
            return default;
        }
    }
}
