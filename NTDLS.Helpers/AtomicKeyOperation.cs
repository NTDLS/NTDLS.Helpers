namespace NTDLS.Helpers
{
    /// <summary>
    /// Allows us to get exclusive thread access for a given string value.
    /// 
    /// Per-key mutual exclusion: all Execute("foo", ...) calls for the same key run one at a time.
    /// Different keys run concurrently.
    /// Keys are removed from the dictionary when no threads are currently in flight for that key, so they don't accumulate forever.
    /// If the user code throws, the lock is released, the exception bubbles up, and the bookkeeping is still correct.
    /// </summary>
    public class AtomicKeyOperation
    {
        private readonly Dictionary<string, VolatileReferenceCounter> _locks = new();

        private class VolatileReferenceCounter
        {
            private long _count = 0;

            public long Count
                => Volatile.Read(ref _count);

            public void Increment()
                => Interlocked.Increment(ref _count);

            public void Decrement()
                => Interlocked.Decrement(ref _count);
        }

        /// <summary>
        /// Executes the specified function within a lock that is unique to the given key, ensuring that only one thread
        /// can execute a function for the same key at a time.
        /// </summary>
        /// <remarks>This method provides per-key synchronization, allowing concurrent execution for
        /// different keys while serializing execution for the same key. The function should not perform long-running or
        /// blocking operations to avoid holding the lock for extended periods.</remarks>
        /// <typeparam name="T">The type of the value returned by the function.</typeparam>
        /// <param name="key">A string that identifies the lock scope. Functions executed with the same key are synchronized; functions
        /// with different keys can run concurrently.</param>
        /// <param name="function">The function to execute within the lock. This delegate is invoked while holding the lock associated with the
        /// specified key.</param>
        /// <returns>The value returned by the executed function.</returns>
        public T Execute<T>(string key, Func<T> function)
        {
            VolatileReferenceCounter? referenceCounter;

            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out referenceCounter))
                {
                    referenceCounter = new VolatileReferenceCounter();
                    _locks.Add(key, referenceCounter);
                }

                referenceCounter.Increment();
            }

            T result;

            try
            {
                lock (referenceCounter)
                {
                    result = function();
                }
            }
            finally
            {
                lock (_locks)
                {
                    referenceCounter.Decrement();
                    if (referenceCounter.Count == 0)
                    {
                        _locks.Remove(key);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes the specified action while ensuring that only one thread can execute an action associated with the
        /// same key at a time.
        /// </summary>
        /// <remarks>This method provides a keyed locking mechanism, allowing concurrent execution for
        /// different keys while serializing execution for the same key. If multiple threads call this method with the
        /// same key, their actions will not overlap. If <paramref name="function"/> is null, an exception will be
        /// thrown.</remarks>
        /// <param name="key">The key that identifies the lock scope. Actions with the same key are executed sequentially; actions with
        /// different keys may execute concurrently.</param>
        /// <param name="function">The action to execute within the lock. Cannot be null.</param>
        public void Execute(string key, Action function)
        {
            VolatileReferenceCounter? referenceCounter;

            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out referenceCounter))
                {
                    referenceCounter = new VolatileReferenceCounter();
                    _locks.Add(key, referenceCounter);
                }

                referenceCounter.Increment();
            }

            try
            {
                lock (referenceCounter)
                {
                    function();
                }
            }
            finally
            {
                lock (_locks)
                {
                    referenceCounter.Decrement();
                    if (referenceCounter.Count == 0)
                    {
                        _locks.Remove(key);
                    }
                }
            }
        }
    }
}
