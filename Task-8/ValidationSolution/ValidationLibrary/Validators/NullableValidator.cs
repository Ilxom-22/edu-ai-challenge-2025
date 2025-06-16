using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for nullable value types.
    /// </summary>
    /// <typeparam name="T">The underlying value type</typeparam>
    public sealed class NullableValidator<T> : BaseValidator<T?> where T : struct
    {
        private readonly IValidator<T> _valueValidator;

        /// <summary>
        /// Initializes a new instance of the NullableValidator class.
        /// </summary>
        /// <param name="valueValidator">The validator for the non-null value</param>
        public NullableValidator(IValidator<T> valueValidator)
        {
            _valueValidator = valueValidator ?? throw new System.ArgumentNullException(nameof(valueValidator));
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new NullableValidator instance</returns>
        protected override BaseValidator<T?> CreateInstance() => new NullableValidator<T>(_valueValidator);

        /// <summary>
        /// Validates the nullable value.
        /// </summary>
        /// <param name="value">The nullable value to validate</param>
        /// <returns>A validation result</returns>
        public override ValidationResult Validate(T? value)
        {
            var baseResult = base.Validate(value);
            if (!baseResult.IsValid)
                return baseResult;

            if (value.HasValue)
                return _valueValidator.Validate(value.Value);

            return ValidationResult.Success();
        }

        /// <summary>
        /// Requires the nullable value to have a value (not be null).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public NullableValidator<T> Required(string message = "Value is required.")
        {
            return (NullableValidator<T>)AddRule(value => value.HasValue, message, "NULLABLE_REQUIRED");
        }
    }

    /// <summary>
    /// Validator for optional reference types (nullable reference types).
    /// </summary>
    /// <typeparam name="T">The reference type</typeparam>
    public sealed class OptionalValidator<T> : BaseValidator<T?> where T : class
    {
        private readonly IValidator<T> _valueValidator;

        /// <summary>
        /// Initializes a new instance of the OptionalValidator class.
        /// </summary>
        /// <param name="valueValidator">The validator for the non-null value</param>
        public OptionalValidator(IValidator<T> valueValidator)
        {
            _valueValidator = valueValidator ?? throw new System.ArgumentNullException(nameof(valueValidator));
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new OptionalValidator instance</returns>
        protected override BaseValidator<T?> CreateInstance() => new OptionalValidator<T>(_valueValidator);

        /// <summary>
        /// Validates the optional value.
        /// </summary>
        /// <param name="value">The optional value to validate</param>
        /// <returns>A validation result</returns>
        public override ValidationResult Validate(T? value)
        {
            var baseResult = base.Validate(value);
            if (!baseResult.IsValid)
                return baseResult;

            if (value != null)
                return _valueValidator.Validate(value);

            return ValidationResult.Success();
        }

        /// <summary>
        /// Requires the optional value to have a value (not be null).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public OptionalValidator<T> Required(string message = "Value is required.")
        {
            return (OptionalValidator<T>)AddRule(value => value != null, message, "OPTIONAL_REQUIRED");
        }
    }
} 