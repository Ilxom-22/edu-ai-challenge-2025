namespace ValidationLibrary.Core
{
    /// <summary>
    /// Exception thrown when validation fails and ValidateAndThrow is called.
    /// </summary>
    public sealed class ValidationException : Exception
    {
        /// <summary>
        /// Gets the validation result that caused this exception.
        /// </summary>
        public ValidationResult ValidationResult { get; }

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IReadOnlyList<ValidationError> Errors => ValidationResult.Errors;

        /// <summary>
        /// Initializes a new instance of the ValidationException class.
        /// </summary>
        /// <param name="validationResult">The validation result that caused this exception</param>
        public ValidationException(ValidationResult validationResult)
            : base(CreateMessage(validationResult))
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class.
        /// </summary>
        /// <param name="validationResult">The validation result that caused this exception</param>
        /// <param name="message">A custom error message</param>
        public ValidationException(ValidationResult validationResult, string message)
            : base(message)
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class.
        /// </summary>
        /// <param name="validationResult">The validation result that caused this exception</param>
        /// <param name="message">A custom error message</param>
        /// <param name="innerException">The inner exception</param>
        public ValidationException(ValidationResult validationResult, string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Creates a default error message from the validation result.
        /// </summary>
        /// <param name="validationResult">The validation result</param>
        /// <returns>A formatted error message</returns>
        private static string CreateMessage(ValidationResult validationResult)
        {
            if (validationResult?.Errors == null || validationResult.Errors.Count == 0)
                return "Validation failed.";

            if (validationResult.Errors.Count == 1)
                return $"Validation failed: {validationResult.Errors[0].Message}";

            return $"Validation failed with {validationResult.Errors.Count} errors: {string.Join("; ", validationResult.Errors.Select(e => e.Message))}";
        }
    }
} 