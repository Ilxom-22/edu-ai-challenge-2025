using System.Text.RegularExpressions;
using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for string values with fluent API support.
    /// </summary>
    public sealed class StringValidator : BaseValidator<string?>
    {
        /// <summary>
        /// Initializes a new instance of the StringValidator class.
        /// </summary>
        public StringValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new StringValidator instance</returns>
        protected override BaseValidator<string?> CreateInstance() => new StringValidator();

        /// <summary>
        /// Validates that the string is not null.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator NotNull(string message = "Value cannot be null.")
        {
            return (StringValidator)AddRule(value => value != null, message, "STRING_NOT_NULL");
        }

        /// <summary>
        /// Validates that the string is not null or empty.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator NotEmpty(string message = "Value cannot be null or empty.")
        {
            return (StringValidator)AddRule(value => !string.IsNullOrEmpty(value), message, "STRING_NOT_EMPTY");
        }

        /// <summary>
        /// Validates that the string is not null, empty, or whitespace.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator NotWhiteSpace(string message = "Value cannot be null, empty, or whitespace.")
        {
            return (StringValidator)AddRule(value => !string.IsNullOrWhiteSpace(value), message, "STRING_NOT_WHITESPACE");
        }

        /// <summary>
        /// Validates that the string has a minimum length.
        /// </summary>
        /// <param name="minLength">The minimum length</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator MinLength(int minLength, string? message = null)
        {
            message ??= $"Value must be at least {minLength} characters long.";
            return (StringValidator)AddRule(value => string.IsNullOrEmpty(value) || value.Length >= minLength, message, "STRING_MIN_LENGTH");
        }

        /// <summary>
        /// Validates that the string has a maximum length.
        /// </summary>
        /// <param name="maxLength">The maximum length</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator MaxLength(int maxLength, string? message = null)
        {
            message ??= $"Value must be at most {maxLength} characters long.";
            return (StringValidator)AddRule(value => value?.Length <= maxLength, message, "STRING_MAX_LENGTH");
        }

        /// <summary>
        /// Validates that the string has an exact length.
        /// </summary>
        /// <param name="length">The exact length</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Length(int length, string? message = null)
        {
            message ??= $"Value must be exactly {length} characters long.";
            return (StringValidator)AddRule(value => value?.Length == length, message, "STRING_EXACT_LENGTH");
        }

        /// <summary>
        /// Validates that the string length is within a specified range.
        /// </summary>
        /// <param name="minLength">The minimum length</param>
        /// <param name="maxLength">The maximum length</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Length(int minLength, int maxLength, string? message = null)
        {
            message ??= $"Value must be between {minLength} and {maxLength} characters long.";
            return (StringValidator)AddRule(
                value => value != null && value.Length >= minLength && value.Length <= maxLength,
                message,
                "STRING_LENGTH_RANGE");
        }

        /// <summary>
        /// Validates that the string matches a regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regular expression pattern</param>
        /// <param name="message">Custom error message</param>
        /// <param name="options">Regular expression options</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Matches(string pattern, string? message = null, RegexOptions options = RegexOptions.None)
        {
            message ??= $"Value must match the pattern: {pattern}";
            var regex = new Regex(pattern, options);
            return (StringValidator)AddRule(value => value != null && regex.IsMatch(value), message, "STRING_REGEX");
        }

        /// <summary>
        /// Validates that the string matches a regular expression.
        /// </summary>
        /// <param name="regex">The regular expression</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Matches(Regex regex, string? message = null)
        {
            message ??= $"Value must match the pattern: {regex}";
            return (StringValidator)AddRule(value => value != null && regex.IsMatch(value), message, "STRING_REGEX");
        }

        /// <summary>
        /// Validates that the string contains a specific substring.
        /// </summary>
        /// <param name="substring">The substring to search for</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparison">String comparison options</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Contains(string substring, string? message = null, StringComparison comparison = StringComparison.Ordinal)
        {
            message ??= $"Value must contain: {substring}";
            return (StringValidator)AddRule(
                value => value != null && value.Contains(substring, comparison),
                message,
                "STRING_CONTAINS");
        }

        /// <summary>
        /// Validates that the string starts with a specific prefix.
        /// </summary>
        /// <param name="prefix">The prefix</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparison">String comparison options</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator StartsWith(string prefix, string? message = null, StringComparison comparison = StringComparison.Ordinal)
        {
            message ??= $"Value must start with: {prefix}";
            return (StringValidator)AddRule(
                value => value != null && value.StartsWith(prefix, comparison),
                message,
                "STRING_STARTS_WITH");
        }

        /// <summary>
        /// Validates that the string ends with a specific suffix.
        /// </summary>
        /// <param name="suffix">The suffix</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparison">String comparison options</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator EndsWith(string suffix, string? message = null, StringComparison comparison = StringComparison.Ordinal)
        {
            message ??= $"Value must end with: {suffix}";
            return (StringValidator)AddRule(
                value => value != null && value.EndsWith(suffix, comparison),
                message,
                "STRING_ENDS_WITH");
        }

        /// <summary>
        /// Validates that the string is a valid email address.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Email(string message = "Value must be a valid email address.")
        {
            var emailRegex = new Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return (StringValidator)AddRule(value => value != null && emailRegex.IsMatch(value), message, "STRING_EMAIL");
        }

        /// <summary>
        /// Validates that the string is a valid URL.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Url(string message = "Value must be a valid URL.")
        {
            return (StringValidator)AddRule(value => value != null && Uri.TryCreate(value, UriKind.Absolute, out _), message, "STRING_URL");
        }

        /// <summary>
        /// Validates that the string represents a valid GUID.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator Guid(string message = "Value must be a valid GUID.")
        {
            return (StringValidator)AddRule(value => value != null && System.Guid.TryParse(value, out _), message, "STRING_GUID");
        }

        /// <summary>
        /// Validates that the string is one of the specified allowed values.
        /// </summary>
        /// <param name="allowedValues">The allowed values</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparison">String comparison options</param>
        /// <returns>This validator instance for method chaining</returns>
        public StringValidator OneOf(string[] allowedValues, string? message = null, StringComparison comparison = StringComparison.Ordinal)
        {
            message ??= $"Value must be one of: {string.Join(", ", allowedValues)}";
            return (StringValidator)AddRule(
                value => value != null && Array.Exists(allowedValues, allowed => string.Equals(value, allowed, comparison)),
                message,
                "STRING_ONE_OF");
        }
    }
} 