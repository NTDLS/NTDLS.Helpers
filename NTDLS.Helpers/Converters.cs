using System.Globalization;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for type conversions.
    /// </summary>
    public class Converters
    {
        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// Returns defaultValue if the value is null or if conversion fails.
        /// </summary>
        public static T ConvertTo<T>(string? value, T defaultValue, CultureInfo? culture = null)
        {
            try
            {
                return ConvertToNullable<T>(value, culture) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// Returns null if the value is null, throws if conversion fails.
        /// </summary>
        public static T? ConvertToNullable<T>(string? value, CultureInfo? culture = null)
        {
            if (value == null)
            {
                return default;
            }

            var targetType = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            var parsed = ParseValue(targetType, value, culture ?? CultureInfo.InvariantCulture);
            return (T?)Convert.ChangeType(parsed, targetType, culture ?? CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Makes a best effort conversion from a string to the given type. Throws if the
        /// value is null or if conversion fails.
        /// </summary>
        public static T ConvertTo<T>(string value, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(value);

            var targetType = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            var parsed = ParseValue(targetType, value, culture ?? CultureInfo.InvariantCulture);
            return (T)Convert.ChangeType(parsed, targetType, culture ?? CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses the given string into an instance of targetType, throwing a consistent
        /// exception on failure. targetType must already be de-nullabled by the caller.
        /// </summary>
        private static object ParseValue(Type targetType, string value, CultureInfo culture)
        {
            try
            {
                if (targetType == typeof(char))
                {
                    if (value.Length != 1)
                    {
                        throw new FormatException($"Value [{value}] is not a single character.");
                    }
                    return value[0];
                }
                else if (targetType == typeof(bool))
                {
                    var boolValue = value.Replace(",", "").Trim().ToLowerInvariant();

                    if (boolValue.Length > 0 && boolValue.All(char.IsNumber))
                    {
                        boolValue = int.Parse(boolValue, culture) != 0 ? "true" : "false";
                    }

                    if (bool.TryParse(boolValue, out var parsedResult) == false)
                    {
                        throw new FormatException($"Value [{value}] is not a valid boolean.");
                    }
                    return parsedResult;
                }
                else if (targetType == typeof(string))
                    return value;
                else if (targetType == typeof(byte))
                    return byte.Parse(value, culture);
                else if (targetType == typeof(sbyte))
                    return sbyte.Parse(value, culture);
                else if (targetType == typeof(short))
                    return short.Parse(value, culture);
                else if (targetType == typeof(ushort))
                    return ushort.Parse(value, culture);
                else if (targetType == typeof(int))
                    return int.Parse(value, culture);
                else if (targetType == typeof(uint))
                    return uint.Parse(value, culture);
                else if (targetType == typeof(long))
                    return long.Parse(value, culture);
                else if (targetType == typeof(ulong))
                    return ulong.Parse(value, culture);
                else if (targetType == typeof(float))
                    return float.Parse(value, culture);
                else if (targetType == typeof(double))
                    return double.Parse(value, culture);
                else if (targetType == typeof(decimal))
                    return decimal.Parse(value, culture);
                else if (targetType == typeof(Guid))
                    return Guid.Parse(value);
                else if (targetType == typeof(DateTime))
                    return DateTime.Parse(value, culture);
                else if (targetType == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value, culture);
                else if (targetType == typeof(TimeSpan))
                    return TimeSpan.Parse(value, culture);
                else if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);

                throw new NotSupportedException($"Unsupported conversion type: [{targetType.Name}].");
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error converting value [{value}] to {targetType.Name}.", ex);
            }
        }
    }
}
