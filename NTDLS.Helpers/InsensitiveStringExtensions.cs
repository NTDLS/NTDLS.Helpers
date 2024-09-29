using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NTDLS.Helpers
{
    /// <summary>
    /// String extensions for handling case-insensitive operations.
    /// </summary>
    public static class InsensitiveString
    {
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private static readonly MemoryCacheEntryOptions _oneMinuteSlidingExpiration
            = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));

        /// <summary>
        /// Returns true if the strings are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this string value, string? otherValue)
        {
            return string.Equals(value, otherValue, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if the strings are not equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNot(this string value, string? otherValue)
        {
            return !string.Equals(value, otherValue, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if the string is in the given array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf(this string value, string?[] otherValues)
        {
            foreach (var otherValue in otherValues)
            {
                if (string.Equals(value, otherValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the string contains the given value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsInsensitive(this string value, string otherValue)
        {
            return value.Contains(otherValue, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if the string contains one of the given values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsInsensitive(this string value, string[] otherValues)
        {
            foreach (var otherValue in otherValues)
            {
                if (value.Contains(otherValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the string matches the given pattern.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLike(this string value, string pattern)
        {
            string cacheKey = $"IsMatchLike:{pattern}";

            if (_cache.TryGetValue<Regex>(cacheKey, out var regex) == false)
            {
                regex = new Regex("^" + Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".") + "$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                _cache.Set(cacheKey, regex, _oneMinuteSlidingExpiration);
            }

            ArgumentNullException.ThrowIfNull(regex);

            return regex.IsMatch(value);
        }

        /// <summary>
        /// Returns true if the string matches one of the the given patterns.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLikeOneOf(this string value, string[] otherValues)
        {
            foreach (var otherValue in otherValues)
            {
                if (value.Contains(otherValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}