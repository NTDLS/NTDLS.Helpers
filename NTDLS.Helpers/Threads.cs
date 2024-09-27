namespace NTDLS.Helpers
{
    /// <summary>
    /// Various thread helpers.
    /// </summary>
    public class Threads
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
        public static void StartThread<T>(T param, StartThreadDelegate<T> proc)
            => new Thread(() => proc(param)).Start();

        /// <summary>
        /// Starts a thread with the given delegate and no parameters.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to pass to the delegate.</typeparam>
        /// <param name="proc">Delegate function to call within the new thread.</param>
        public static void StartThread(StartThreadDelegate proc)
            => new Thread(() => proc()).Start();
    }
}
