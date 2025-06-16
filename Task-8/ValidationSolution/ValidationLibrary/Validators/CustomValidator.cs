using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// A validator that uses a custom validation function.
    /// </summary>
    /// <typeparam name="T">The type being validated</typeparam>
    public sealed class CustomValidator<T> : BaseValidator<T>
    {
        private readonly Func<T, ValidationResult> _validationFunction;

        /// <summary>
        /// Initializes a new instance of the CustomValidator class.
        /// </summary>
        /// <param name="validationFunction">The validation function</param>
        public CustomValidator(Func<T, ValidationResult> validationFunction)
        {
            _validationFunction = validationFunction ?? throw new ArgumentNullException(nameof(validationFunction));
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new CustomValidator instance</returns>
        protected override BaseValidator<T> CreateInstance() => new CustomValidator<T>(_validationFunction);

        /// <summary>
        /// Validates the specified value using the custom validation function.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result</returns>
        public override ValidationResult Validate(T value)
        {
            try
            {
                var customResult = _validationFunction(value);
                var baseResult = base.Validate(value);
                return baseResult.And(customResult);
            }
            catch (Exception ex)
            {
                return ValidationResult.Failure($"Custom validation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds an additional validation rule to this custom validator.
        /// </summary>
        /// <param name="additionalFunction">The additional validation function</param>
        /// <returns>A new CustomValidator with the combined validation</returns>
        public CustomValidator<T> And(Func<T, ValidationResult> additionalFunction)
        {
            return new CustomValidator<T>(value =>
            {
                var result1 = _validationFunction(value);
                var result2 = additionalFunction(value);
                return result1.And(result2);
            });
        }

        /// <summary>
        /// Adds an additional validation rule with a predicate and error message.
        /// </summary>
        /// <param name="predicate">The validation predicate</param>
        /// <param name="errorMessage">The error message if validation fails</param>
        /// <param name="errorCode">Optional error code</param>
        /// <returns>A new CustomValidator with the combined validation</returns>
        public CustomValidator<T> And(Func<T, bool> predicate, string errorMessage, string? errorCode = null)
        {
            return And(value => predicate(value)
                ? ValidationResult.Success()
                : ValidationResult.Failure(new[] { new ValidationError(errorMessage, errorCode: errorCode) }));
        }
    }
} 