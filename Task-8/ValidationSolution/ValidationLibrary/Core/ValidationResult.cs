namespace ValidationLibrary.Core
{
    /// <summary>
    /// Represents the outcome of a validation operation.
    /// Contains information about success/failure and any validation errors.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the validation was successful.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the collection of validation errors. Empty if validation was successful.
        /// </summary>
        public IReadOnlyList<ValidationError> Errors { get; }

        /// <summary>
        /// Gets the first error message, or null if validation was successful.
        /// </summary>
        public string? ErrorMessage => Errors.FirstOrDefault()?.Message;

        /// <summary>
        /// Initializes a new instance of the ValidationResult class for a successful validation.
        /// </summary>
        private ValidationResult()
        {
            IsValid = true;
            Errors = Array.Empty<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the ValidationResult class for a failed validation.
        /// </summary>
        /// <param name="errors">The validation errors</param>
        private ValidationResult(IEnumerable<ValidationError> errors)
        {
            var errorList = errors?.ToList() ?? new List<ValidationError>();
            IsValid = errorList.Count == 0;
            Errors = errorList.AsReadOnly();
        }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        /// <returns>A successful validation result</returns>
        public static ValidationResult Success() => new();

        /// <summary>
        /// Creates a failed validation result with a single error.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="propertyName">The name of the property that failed validation</param>
        /// <returns>A failed validation result</returns>
        public static ValidationResult Failure(string message, string? propertyName = null)
            => new(new[] { new ValidationError(message, propertyName) });

        /// <summary>
        /// Creates a failed validation result with multiple errors.
        /// </summary>
        /// <param name="errors">The validation errors</param>
        /// <returns>A failed validation result</returns>
        public static ValidationResult Failure(IEnumerable<ValidationError> errors)
            => new(errors);

        /// <summary>
        /// Creates a failed validation result with multiple error messages.
        /// </summary>
        /// <param name="messages">The error messages</param>
        /// <param name="propertyName">The name of the property that failed validation</param>
        /// <returns>A failed validation result</returns>
        public static ValidationResult Failure(IEnumerable<string> messages, string? propertyName = null)
            => new(messages.Select(m => new ValidationError(m, propertyName)));

        /// <summary>
        /// Combines multiple validation results into a single result.
        /// </summary>
        /// <param name="results">The validation results to combine</param>
        /// <returns>A combined validation result</returns>
        public static ValidationResult Combine(params ValidationResult[] results)
        {
            var allErrors = results.SelectMany(r => r.Errors).ToList();
            return allErrors.Count == 0 ? Success() : new ValidationResult(allErrors);
        }

        /// <summary>
        /// Combines this validation result with another validation result.
        /// </summary>
        /// <param name="other">The other validation result</param>
        /// <returns>A combined validation result</returns>
        public ValidationResult And(ValidationResult other)
            => Combine(this, other);

        /// <summary>
        /// Returns a string representation of the validation result.
        /// </summary>
        /// <returns>A string representation of the validation result</returns>
        public override string ToString()
        {
            if (IsValid)
                return "Validation succeeded";

            return $"Validation failed: {string.Join("; ", Errors.Select(e => e.Message))}";
        }
    }
} 