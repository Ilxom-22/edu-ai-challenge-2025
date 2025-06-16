namespace ValidationLibrary.Core
{
    /// <summary>
    /// Abstract base class for all validators, providing common functionality.
    /// </summary>
    /// <typeparam name="TValue">The type of value being validated</typeparam>
    public abstract class BaseValidator<TValue> : IValidator<TValue>, IValidator
    {
        private readonly List<Func<TValue, ValidationResult>> _rules = new();

        /// <summary>
        /// Gets the type of value this validator can validate.
        /// </summary>
        public Type ValidatedType => typeof(TValue);

        /// <summary>
        /// Validates the specified value using all configured rules.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result containing success status and any errors</returns>
        public virtual ValidationResult Validate(TValue value)
        {
            var results = new List<ValidationResult>();

            // Run pre-validation logic
            var preValidationResult = PreValidate(value);
            if (!preValidationResult.IsValid)
                return preValidationResult;

            // Run all rules
            foreach (var rule in _rules)
            {
                var result = rule(value);
                results.Add(result);
            }

            // Run post-validation logic
            var combinedResult = ValidationResult.Combine(results.ToArray());
            return PostValidate(value, combinedResult);
        }

        /// <summary>
        /// Validates the specified value and throws an exception if validation fails.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>The validated value</returns>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public virtual TValue ValidateAndThrow(TValue value)
        {
            var result = Validate(value);
            if (!result.IsValid)
                throw new ValidationException(result);
            return value;
        }

        /// <summary>
        /// Attempts to validate the specified value without throwing exceptions.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="result">The validation result</param>
        /// <returns>True if validation succeeded, false otherwise</returns>
        public virtual bool TryValidate(TValue value, out ValidationResult result)
        {
            try
            {
                result = Validate(value);
                return result.IsValid;
            }
            catch
            {
                result = ValidationResult.Failure("An unexpected error occurred during validation.");
                return false;
            }
        }

        /// <summary>
        /// Non-generic validation method for runtime type validation.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result containing success status and any errors</returns>
        public ValidationResult Validate(object? value)
        {
            if (value is TValue typedValue)
                return Validate(typedValue);

            if (value is null && !typeof(TValue).IsValueType)
                return Validate(default(TValue)!);

            return ValidationResult.Failure($"Expected value of type {typeof(TValue).Name}, but got {value?.GetType().Name ?? "null"}.");
        }

        /// <summary>
        /// Adds a validation rule to this validator.
        /// </summary>
        /// <param name="rule">The validation rule to add</param>
        /// <returns>This validator instance for method chaining</returns>
        protected BaseValidator<TValue> AddRule(Func<TValue, ValidationResult> rule)
        {
            _rules.Add(rule ?? throw new ArgumentNullException(nameof(rule)));
            return this;
        }

        /// <summary>
        /// Adds a simple validation rule with a predicate and error message.
        /// </summary>
        /// <param name="predicate">The predicate to test the value</param>
        /// <param name="errorMessage">The error message if validation fails</param>
        /// <param name="errorCode">Optional error code for programmatic error handling</param>
        /// <returns>This validator instance for method chaining</returns>
        protected BaseValidator<TValue> AddRule(Func<TValue, bool> predicate, string errorMessage, string? errorCode = null)
        {
            return AddRule(value => predicate(value) 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(new[] { new ValidationError(errorMessage, errorCode: errorCode) }));
        }

        /// <summary>
        /// Called before running validation rules. Override to implement custom pre-validation logic.
        /// </summary>
        /// <param name="value">The value being validated</param>
        /// <returns>A validation result. If not successful, validation will stop</returns>
        protected virtual ValidationResult PreValidate(TValue value)
        {
            return ValidationResult.Success();
        }

        /// <summary>
        /// Called after running validation rules. Override to implement custom post-validation logic.
        /// </summary>
        /// <param name="value">The value being validated</param>
        /// <param name="result">The combined result of all validation rules</param>
        /// <returns>The final validation result</returns>
        protected virtual ValidationResult PostValidate(TValue value, ValidationResult result)
        {
            return result;
        }

        /// <summary>
        /// Creates a copy of this validator with the same rules.
        /// </summary>
        /// <returns>A copy of this validator</returns>
        public virtual BaseValidator<TValue> Clone()
        {
            var clone = CreateInstance();
            clone._rules.AddRange(_rules);
            return clone;
        }

        /// <summary>
        /// Creates a new instance of this validator type. Override in derived classes.
        /// </summary>
        /// <returns>A new instance of this validator</returns>
        protected abstract BaseValidator<TValue> CreateInstance();
    }
} 