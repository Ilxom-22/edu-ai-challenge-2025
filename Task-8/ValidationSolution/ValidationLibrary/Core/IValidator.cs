namespace ValidationLibrary.Core
{
    /// <summary>
    /// Defines the contract for all validators in the validation library.
    /// Provides type-safe validation with comprehensive error reporting.
    /// </summary>
    /// <typeparam name="TValue">The type of value being validated</typeparam>
    public interface IValidator<TValue>
    {
        /// <summary>
        /// Validates the specified value and returns a validation result.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result containing success status and any errors</returns>
        ValidationResult Validate(TValue value);

        /// <summary>
        /// Validates the specified value and throws an exception if validation fails.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>The validated value</returns>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        TValue ValidateAndThrow(TValue value);

        /// <summary>
        /// Attempts to validate the specified value without throwing exceptions.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="result">The validation result if successful</param>
        /// <returns>True if validation succeeded, false otherwise</returns>
        bool TryValidate(TValue value, out ValidationResult result);
    }

    /// <summary>
    /// Non-generic interface for validators to support runtime type validation.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Gets the type of value this validator can validate.
        /// </summary>
        Type ValidatedType { get; }

        /// <summary>
        /// Validates the specified value and returns a validation result.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result containing success status and any errors</returns>
        ValidationResult Validate(object? value);
    }
} 