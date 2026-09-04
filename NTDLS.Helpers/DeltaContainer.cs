using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Keeps track of values between multiple observations, identified by a key and allows calculating deltas and rates.
    /// </summary>
    public class DeltaContainer<T> : IDisposable where T : INumber<T>
    {
        private readonly MemoryCache _collection = new(new MemoryCacheOptions());
        private readonly MemoryCacheEntryOptions _slidingExpiration;
        private readonly object _syncRoot = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the DeltaContainer class with the specified retention duration for cached
        /// items.
        /// </summary>
        /// <param name="retentionDuration">The duration for which each item remains in the cache since its last access. Must be a positive time
        /// interval.</param>
        public DeltaContainer(TimeSpan retentionDuration)
        {
            _slidingExpiration = new MemoryCacheEntryOptions
            {
                SlidingExpiration = retentionDuration
            };
        }

        /// <summary>
        /// Represents a value of type T associated with a specific UTC timestamp.
        /// </summary>
        /// <param name="timestampUTC">The point in time, expressed in Coordinated Universal Time (UTC), when the value was recorded.</param>
        /// <param name="actualValue">The actual value associated with the specified timestamp.</param>
        public class DeltaContainerValue(DateTime timestampUTC, T actualValue)
        {
            /// <summary>
            /// Gets the date and time, in Coordinated Universal Time (UTC), when the event occurred.
            /// </summary>
            public DateTime TimestampUTC { get; private set; } = timestampUTC;
            /// <summary>
            /// Gets the actual value represented by this instance.
            /// </summary>
            public T Actual { get; private set; } = actualValue;
        }

        #region Calculate Delta.

        /// <summary>
        /// Attempts to calculate the delta between the current value and the previous value for the given key.
        /// Returns null if no previous value exists, or if the value has decreased since the previous observation.
        /// </summary>
        public T? CalculateDelta(string key, DateTime timestampUTC, T actualValue)
            => TryCalculateDelta(key, new(timestampUTC, actualValue), out var delta) ? delta : default;

        /// <summary>
        /// Attempts to calculate the delta between the current value and the previous value for the given key.
        /// Returns false if no previous value exists, or if the value has decreased since the previous observation.
        /// </summary>
        public bool TryCalculateDelta(string key, DateTime timestampUTC, T actualValue, [NotNullWhen(true)] out T? delta)
            => TryCalculateDelta(key, new(timestampUTC, actualValue), out delta);

        /// <summary>
        /// Attempts to calculate the delta between the current value and the previous value for the given key.
        /// </summary>
        private bool TryCalculateDelta(string key, DeltaContainerValue given, [NotNullWhen(true)] out T? delta)
        {
            lock (_syncRoot)
            {
                if (_collection.TryGetValue<DeltaContainerValue>(key, out var previous) && previous != null)
                {
                    var diff = given.Actual - previous.Actual;
                    if (diff >= T.Zero)
                    {
                        delta = diff;
                        _collection.Set(key, given, _slidingExpiration);
                        return true;
                    }
                }

                _collection.Set(key, given, _slidingExpiration);
                delta = default;
                return false;
            }
        }

        #endregion

        #region Calculate Per-Second.

        /// <summary>
        /// Attempts to calculate the delta/s between the current value and the previous value for the given key.
        /// Returns null if no previous value exists, if the timestamp has not advanced, or if the value has
        /// decreased since the previous observation.
        /// </summary>
        public double? CalculatePerSecond(string key, DateTime timestampUTC, T actualValue)
            => TryCalculatePerSecond(key, new(timestampUTC, actualValue), out var delta) ? delta : null;

        /// <summary>
        /// Attempts to calculate the delta/s between the current value and the previous value for the given key.
        /// Returns false if no previous value exists, if the timestamp has not advanced, or if the value has
        /// decreased since the previous observation.
        /// </summary>
        public bool TryCalculatePerSecond(string key, DateTime timestampUTC, T actualValue, [NotNullWhen(true)] out double? delta)
            => TryCalculatePerSecond(key, new(timestampUTC, actualValue), out delta);

        /// <summary>
        /// Attempts to calculate the delta/s between the current value and the previous value for the given key.
        /// </summary>
        private bool TryCalculatePerSecond(string key, DeltaContainerValue given, [NotNullWhen(true)] out double? delta)
        {
            lock (_syncRoot)
            {
                if (_collection.TryGetValue<DeltaContainerValue>(key, out var previous) && previous != null)
                {
                    var deltaSeconds = (given.TimestampUTC - previous.TimestampUTC).TotalSeconds;
                    var diff = given.Actual - previous.Actual;
                    if (deltaSeconds > 0 && diff >= T.Zero)
                    {
                        delta = double.CreateChecked(diff) / deltaSeconds;
                        _collection.Set(key, given, _slidingExpiration);
                        return true;
                    }
                }

                _collection.Set(key, given, _slidingExpiration);
                delta = null;
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Releases the resources used by the underlying cache.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _collection.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
