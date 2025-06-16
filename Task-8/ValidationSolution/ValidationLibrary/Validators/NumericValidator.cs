using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Generic validator for numeric values with fluent API support.
    /// </summary>
    /// <typeparam name="T">The numeric type being validated</typeparam>
    public sealed class NumericValidator<T> : BaseValidator<T> where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Initializes a new instance of the NumericValidator class.
        /// </summary>
        public NumericValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new NumericValidator instance</returns>
        protected override BaseValidator<T> CreateInstance() => new NumericValidator<T>();

        /// <summary>
        /// Validates that the value is greater than the specified minimum.
        /// </summary>
        /// <param name="min">The minimum value (exclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> GreaterThan(T min, string? message = null)
        {
            message ??= $"Value must be greater than {min}.";
            return (NumericValidator<T>)AddRule(value => value.CompareTo(min) > 0, message, "NUMERIC_GREATER_THAN");
        }

        /// <summary>
        /// Validates that the value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="min">The minimum value (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> GreaterThanOrEqual(T min, string? message = null)
        {
            message ??= $"Value must be greater than or equal to {min}.";
            return (NumericValidator<T>)AddRule(value => value.CompareTo(min) >= 0, message, "NUMERIC_GREATER_THAN_OR_EQUAL");
        }

        /// <summary>
        /// Validates that the value is less than the specified maximum.
        /// </summary>
        /// <param name="max">The maximum value (exclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> LessThan(T max, string? message = null)
        {
            message ??= $"Value must be less than {max}.";
            return (NumericValidator<T>)AddRule(value => value.CompareTo(max) < 0, message, "NUMERIC_LESS_THAN");
        }

        /// <summary>
        /// Validates that the value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="max">The maximum value (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> LessThanOrEqual(T max, string? message = null)
        {
            message ??= $"Value must be less than or equal to {max}.";
            return (NumericValidator<T>)AddRule(value => value.CompareTo(max) <= 0, message, "NUMERIC_LESS_THAN_OR_EQUAL");
        }

        /// <summary>
        /// Validates that the value is within the specified range (inclusive).
        /// </summary>
        /// <param name="min">The minimum value (inclusive)</param>
        /// <param name="max">The maximum value (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> Between(T min, T max, string? message = null)
        {
            message ??= $"Value must be between {min} and {max}.";
            return (NumericValidator<T>)AddRule(
                value => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0,
                message,
                "NUMERIC_BETWEEN");
        }

        /// <summary>
        /// Validates that the value equals the specified value.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> Equal(T expected, string? message = null)
        {
            message ??= $"Value must equal {expected}.";
            return (NumericValidator<T>)AddRule(value => value.Equals(expected), message, "NUMERIC_EQUAL");
        }

        /// <summary>
        /// Validates that the value does not equal the specified value.
        /// </summary>
        /// <param name="forbidden">The forbidden value</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> NotEqual(T forbidden, string? message = null)
        {
            message ??= $"Value must not equal {forbidden}.";
            return (NumericValidator<T>)AddRule(value => !value.Equals(forbidden), message, "NUMERIC_NOT_EQUAL");
        }

        /// <summary>
        /// Validates that the value is one of the specified allowed values.
        /// </summary>
        /// <param name="allowedValues">The allowed values</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> OneOf(IEnumerable<T> allowedValues, string? message = null)
        {
            var allowed = new HashSet<T>(allowedValues);
            message ??= $"Value must be one of: {string.Join(", ", allowed)}";
            return (NumericValidator<T>)AddRule(value => allowed.Contains(value), message, "NUMERIC_ONE_OF");
        }

        /// <summary>
        /// Validates that the value is positive (greater than zero).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> Positive(string message = "Value must be positive.")
        {
            var zero = GetZero();
            return (NumericValidator<T>)AddRule(value => value.CompareTo(zero) > 0, message, "NUMERIC_POSITIVE");
        }

        /// <summary>
        /// Validates that the value is negative (less than zero).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> Negative(string message = "Value must be negative.")
        {
            var zero = GetZero();
            return (NumericValidator<T>)AddRule(value => value.CompareTo(zero) < 0, message, "NUMERIC_NEGATIVE");
        }

        /// <summary>
        /// Validates that the value is zero.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> Zero(string message = "Value must be zero.")
        {
            var zero = GetZero();
            return (NumericValidator<T>)AddRule(value => value.Equals(zero), message, "NUMERIC_ZERO");
        }

        /// <summary>
        /// Validates that the value is not zero.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NumericValidator<T> NotZero(string message = "Value must not be zero.")
        {
            var zero = GetZero();
            return (NumericValidator<T>)AddRule(value => !value.Equals(zero), message, "NUMERIC_NOT_ZERO");
        }

        /// <summary>
        /// Gets the zero value for the numeric type.
        /// </summary>
        /// <returns>The zero value</returns>
        private static T GetZero()
        {
            if (typeof(T) == typeof(int)) return (T)(object)0;
            if (typeof(T) == typeof(long)) return (T)(object)0L;
            if (typeof(T) == typeof(float)) return (T)(object)0f;
            if (typeof(T) == typeof(double)) return (T)(object)0.0;
            if (typeof(T) == typeof(decimal)) return (T)(object)0m;
            if (typeof(T) == typeof(short)) return (T)(object)(short)0;
            if (typeof(T) == typeof(byte)) return (T)(object)(byte)0;
            if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)0;
            if (typeof(T) == typeof(uint)) return (T)(object)0u;
            if (typeof(T) == typeof(ulong)) return (T)(object)0ul;
            if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)0;

            // For other types, try to use default
            return default(T)!;
        }
    }

    /// <summary>
    /// Provides static factory methods for creating numeric validators.
    /// </summary>
    public static class NumericValidator
    {
        /// <summary>
        /// Creates a new integer validator.
        /// </summary>
        /// <returns>A new NumericValidator for integers</returns>
        public static NumericValidator<int> Int() => new();

        /// <summary>
        /// Creates a new long validator.
        /// </summary>
        /// <returns>A new NumericValidator for longs</returns>
        public static NumericValidator<long> Long() => new();

        /// <summary>
        /// Creates a new float validator.
        /// </summary>
        /// <returns>A new NumericValidator for floats</returns>
        public static NumericValidator<float> Float() => new();

        /// <summary>
        /// Creates a new double validator.
        /// </summary>
        /// <returns>A new NumericValidator for doubles</returns>
        public static NumericValidator<double> Double() => new();

        /// <summary>
        /// Creates a new decimal validator.
        /// </summary>
        /// <returns>A new NumericValidator for decimals</returns>
        public static NumericValidator<decimal> Decimal() => new();

        /// <summary>
        /// Creates a new short validator.
        /// </summary>
        /// <returns>A new NumericValidator for shorts</returns>
        public static NumericValidator<short> Short() => new();

        /// <summary>
        /// Creates a new byte validator.
        /// </summary>
        /// <returns>A new NumericValidator for bytes</returns>
        public static NumericValidator<byte> Byte() => new();
    }
} 