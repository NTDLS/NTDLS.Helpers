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
        /// Returns the value of the nullable guid, throws an exception if the value is null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid EnsureNotNull([NotNull] this Guid? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException("Value should not be null: '" + strName + "'.");
            }
            return (Guid)value;
        }

        /// <summary>
        /// Returns the value of the nullable guid, throws an exception if the value is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid EnsureNotNullOrEmpty([NotNull] this Guid? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (!value.HasValue || value == Guid.Empty)
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + strName + "'.");
            }
            return (Guid)value;
        }

        /// <summary>
        /// Returns the value of the guid, throws an exception if the value is empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid EnsureNotNullOrEmpty([NotNull] this Guid value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + strName + "'.");
            }
            return value;
        }

        /// <summary>
        /// Returns the value of the nullable string, throws an exception if the value is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnsureNotNullOrEmpty([NotNull] this string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + strName + "'.");
            }
            return value;
        }

        /// <summary>
        /// Returns the value of the nullable string, throws an exception if the value is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrWhiteSpace([NotNull] this string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("Value should not be null or empty: '" + strName + "'.");
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
