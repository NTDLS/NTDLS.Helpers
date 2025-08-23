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
        /// </summary>
        public static T ConvertTo<T>(string? value, T defaultValue)
            => ConvertToNullable<T>(value) ?? defaultValue;

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T? ConvertToNullable<T>(string? value)
        {
            if (value == null)
            {
                return default;
            }

            var targetType = typeof(T);
            if (Nullable.GetUnderlyingType(targetType) != null)
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType == typeof(string))
            {
                return (T?)Convert.ChangeType(value, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(int))
            {
                if (int.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(ulong))
            {
                if (ulong.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(float))
            {
                if (float.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(double))
            {
                if (double.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(bool))
            {
                value = value.Replace(",", "").ToLower();

                if (value.All(char.IsNumber))
                {
                    value = int.Parse(value, CultureInfo.InvariantCulture) != 0 ? "true" : "false";
                }

                if (bool.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to {targetType.Name}.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull(), CultureInfo.InvariantCulture);
            }
            else if (targetType == typeof(Guid))
            {
                return (T?)Convert.ChangeType(Guid.Parse(value, CultureInfo.InvariantCulture), targetType.EnsureNotNull());
            }
            else if (targetType == typeof(DateTime))
            {
                return (T?)Convert.ChangeType(DateTime.Parse(value, CultureInfo.InvariantCulture), targetType.EnsureNotNull());
            }
            else
            {
                throw new Exception($"Unsupported conversion type: [{targetType?.Name ?? typeof(T).Name}].");
            }
        }

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T ConvertTo<T>(string value)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(ulong))
            {
                if (ulong.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to float.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value, CultureInfo.InvariantCulture, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to double.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(bool))
            {
                value = value.ToLower();

                if (value.All(char.IsNumber))
                {
                    if (int.Parse(value, CultureInfo.InvariantCulture) != 0)
                        value = "true";
                    else
                        value = "false";
                }

                if (bool.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to boolean.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception($"Unsupported conversion type.");
            }
        }
    }
}
