using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for enum values with fluent API support.
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    public sealed class EnumValidator<TEnum> : BaseValidator<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// Initializes a new instance of the EnumValidator class.
        /// </summary>
        public EnumValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new EnumValidator instance</returns>
        protected override BaseValidator<TEnum> CreateInstance() => new EnumValidator<TEnum>();

        /// <summary>
        /// Validates that the enum value is defined (a valid enum value).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> IsDefined(string? message = null)
        {
            message ??= $"Value must be a valid {typeof(TEnum).Name} enum value.";
            return (EnumValidator<TEnum>)AddRule(value => Enum.IsDefined(typeof(TEnum), value), message, "ENUM_IS_DEFINED");
        }

        /// <summary>
        /// Validates that the enum value is one of the specified allowed values.
        /// </summary>
        /// <param name="allowedValues">The allowed enum values</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> OneOf(TEnum[] allowedValues, string? message = null)
        {
            message ??= $"Value must be one of: {string.Join(", ", allowedValues)}";
            return (EnumValidator<TEnum>)AddRule(
                value => allowedValues.Contains(value),
                message,
                "ENUM_ONE_OF");
        }

        /// <summary>
        /// Validates that the enum value is not one of the specified forbidden values.
        /// </summary>
        /// <param name="forbiddenValues">The forbidden enum values</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> NotOneOf(TEnum[] forbiddenValues, string? message = null)
        {
            message ??= $"Value must not be one of: {string.Join(", ", forbiddenValues)}";
            return (EnumValidator<TEnum>)AddRule(
                value => !forbiddenValues.Contains(value),
                message,
                "ENUM_NOT_ONE_OF");
        }

        /// <summary>
        /// Validates that the enum value equals the specified value.
        /// </summary>
        /// <param name="expected">The expected enum value</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> Equal(TEnum expected, string? message = null)
        {
            message ??= $"Value must equal {expected}.";
            return (EnumValidator<TEnum>)AddRule(value => value.Equals(expected), message, "ENUM_EQUAL");
        }

        /// <summary>
        /// Validates that the enum value does not equal the specified value.
        /// </summary>
        /// <param name="forbidden">The forbidden enum value</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> NotEqual(TEnum forbidden, string? message = null)
        {
            message ??= $"Value must not equal {forbidden}.";
            return (EnumValidator<TEnum>)AddRule(value => !value.Equals(forbidden), message, "ENUM_NOT_EQUAL");
        }

        /// <summary>
        /// Validates that the enum has a specific flag set (for [Flags] enums).
        /// </summary>
        /// <param name="flag">The flag that must be set</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> HasFlag(TEnum flag, string? message = null)
        {
            message ??= $"Value must have flag {flag} set.";
            return (EnumValidator<TEnum>)AddRule(
                value => value.HasFlag(flag),
                message,
                "ENUM_HAS_FLAG");
        }

        /// <summary>
        /// Validates that the enum does not have a specific flag set (for [Flags] enums).
        /// </summary>
        /// <param name="flag">The flag that must not be set</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public EnumValidator<TEnum> DoesNotHaveFlag(TEnum flag, string? message = null)
        {
            message ??= $"Value must not have flag {flag} set.";
            return (EnumValidator<TEnum>)AddRule(
                value => !value.HasFlag(flag),
                message,
                "ENUM_DOES_NOT_HAVE_FLAG");
        }
    }
} 