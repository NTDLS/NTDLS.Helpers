namespace NTDLS.Helpers
{
    /// <summary>
    /// Various thread helpers.
    /// </summary>
    public class Threading
    {
        /// <summary>
        /// Delegate used for StartThread().
        /// </summary>
        public delegate void StartThreadDelegate();

        /// <summary>
        /// Delegate used for StartThread().
        /// </summary>
        public delegate void StartThreadDelegate<T>(T proc);

        /// <summary>
        /// Starts a thread with the given delegate and parameter.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to pass to the delegate.</typeparam>
        /// <param name="param">Parameter to pass to delegate.</param>
        /// <param name="proc">Delegate function to call the new thread and pass the given parameters.</param>
        public static Thread StartThread<T>(T param, StartThreadDelegate<T> proc)
        {
            var thread = new Thread(() => proc(param));
            thread.Start();
            return thread;
        }

        /// <summary>
        /// Starts a thread with the given delegate and no parameters.
        /// </summary>
        /// <param name="proc">Delegate function to call within the new thread.</param>
        public static Thread StartThread(StartThreadDelegate proc)
        {
            var thread = new Thread(() => proc());
            thread.Start();
            return thread;
        }
    }
}
