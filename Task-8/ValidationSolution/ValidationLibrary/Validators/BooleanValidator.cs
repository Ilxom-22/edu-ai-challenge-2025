using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for boolean values with fluent API support.
    /// </summary>
    public sealed class BooleanValidator : BaseValidator<bool>
    {
        /// <summary>
        /// Initializes a new instance of the BooleanValidator class.
        /// </summary>
        public BooleanValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new BooleanValidator instance</returns>
        protected override BaseValidator<bool> CreateInstance() => new BooleanValidator();

        /// <summary>
        /// Validates that the boolean value is true.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public BooleanValidator IsTrue(string message = "Value must be true.")
        {
            return (BooleanValidator)AddRule(value => value, message, "BOOLEAN_TRUE");
        }

        /// <summary>
        /// Validates that the boolean value is false.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public BooleanValidator IsFalse(string message = "Value must be false.")
        {
            return (BooleanValidator)AddRule(value => !value, message, "BOOLEAN_FALSE");
        }

        /// <summary>
        /// Validates that the boolean value equals the specified value.
        /// </summary>
        /// <param name="expected">The expected boolean value</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public BooleanValidator Equal(bool expected, string? message = null)
        {
            message ??= $"Value must be {expected}.";
            return (BooleanValidator)AddRule(value => value == expected, message, "BOOLEAN_EQUAL");
        }
    }
} 