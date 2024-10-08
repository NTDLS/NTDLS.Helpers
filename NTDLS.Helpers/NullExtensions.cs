﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Extension methods for handling nullable values.
    /// </summary>
    public static class NullExtensions
    {
        /// <summary>
        /// Returns true if the value is default.
        /// </summary>
        public static bool IsDefault<T>(this T? value)
            => value == null || EqualityComparer<T>.Default.Equals(value, default(T));

        /// <summary>
        /// Returns true if the value is NOT default.
        /// </summary>
        public static bool IsNotDefault<T>(this T? value)
            => value != null && !EqualityComparer<T>.Default.Equals(value, default(T));

        /// <summary>
        /// Returns the value of the nullable type, throws an exception if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnsureNotNull<T>([NotNull] this T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName, "Value should not be null.");
                }

                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// Returns the value of the nullable type, throws an exception if the value is null.
        /// </summary>
        public static T EnsureNotNull<T>(this T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName, "Value should not be null.");
                }

                throw new ArgumentException(message, paramName);
            }
            return value.Value;
        }

        /// <summary>
        /// Returns the value of the nullable type with type casting, throws an exception if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnsureNotNull<T>([NotNull] this object? value, string? message = null, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName, "Value should not be null.");
                }

                throw new ArgumentException(message, paramName);
            }
            return (T)value;
        }

        /// <summary>
        /// Returns the value of the nullable type, throws an exception if the value is null.
        /// </summary>
        public static T EnsureNotNullOrEmpty<T>(this T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct
        {
            if (value == null || value.HasValue == false)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("Value should not be null or empty: '" + paramName + "'.");
                }

                throw new ArgumentException(message, paramName);
            }
            return value.Value;
        }

        /// <summary>
        /// Returns the value of the nullable string, throws an exception if the value is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnsureNotNullOrEmpty([NotNull] this string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + paramName + "'.");
            }
            return value;
        }

        /// <summary>
        /// Returns the value of the nullable string, throws an exception if the value is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrWhiteSpace([NotNull] this string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + paramName + "'.");
            }
        }

        /// <summary>
        /// Returns the passed value, or the given default if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DefaultWhenNull<T>(this T? value, T defaultValue)
            => value == null ? defaultValue : value;

        /// <summary>
        /// Returns the passed value, or the given default if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DefaultWhenNullOrEmpty(this string? value, string defaultValue)
            => string.IsNullOrEmpty(value) == true ? defaultValue : value;

        /// <summary>
        /// Returns true if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(this T? value) where T : class
            => value == null;

        /// <summary>
        /// Returns true if the value is null or empty
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value)
            => string.IsNullOrEmpty(value);
    }
}
