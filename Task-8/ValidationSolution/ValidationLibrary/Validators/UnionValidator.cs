using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator that accepts one of multiple possible types or validators.
    /// Similar to discriminated unions in functional programming.
    /// </summary>
    public sealed class UnionValidator : IValidator<object?>, IValidator
    {
        private readonly IValidator[] _validators;

        /// <summary>
        /// Gets the type of value this validator can validate.
        /// </summary>
        public Type ValidatedType => typeof(object);

        /// <summary>
        /// Initializes a new instance of the UnionValidator class.
        /// </summary>
        /// <param name="validators">The validators for each possible type</param>
        public UnionValidator(params IValidator[] validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            if (_validators.Length == 0)
                throw new ArgumentException("At least one validator must be provided.", nameof(validators));
        }

        /// <summary>
        /// Validates the value against all possible validators until one succeeds.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result</returns>
        public ValidationResult Validate(object? value)
        {
            var allErrors = new List<ValidationError>();
            var validatorResults = new List<string>();

            foreach (var validator in _validators)
            {
                try
                {
                    var result = validator.Validate(value);
                    if (result.IsValid)
                        return ValidationResult.Success();

                    allErrors.AddRange(result.Errors);
                    validatorResults.Add($"{validator.ValidatedType.Name}: {result.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    validatorResults.Add($"{validator.ValidatedType.Name}: {ex.Message}");
                }
            }

            var combinedMessage = $"Value does not match any of the expected types. Validation attempts: {string.Join("; ", validatorResults)}";
            return ValidationResult.Failure(combinedMessage);
        }

        /// <summary>
        /// Validates the specified value and throws an exception if validation fails.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>The validated value</returns>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public object? ValidateAndThrow(object? value)
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
        public bool TryValidate(object? value, out ValidationResult result)
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
        /// Adds an additional validator to the union.
        /// </summary>
        /// <param name="validator">The validator to add</param>
        /// <returns>A new UnionValidator with the additional validator</returns>
        public UnionValidator Or(IValidator validator)
        {
            var newValidators = new IValidator[_validators.Length + 1];
            Array.Copy(_validators, newValidators, _validators.Length);
            newValidators[_validators.Length] = validator;
            return new UnionValidator(newValidators);
        }

        /// <summary>
        /// Adds an additional validator for a specific type to the union.
        /// </summary>
        /// <typeparam name="T">The type for the new validator</typeparam>
        /// <param name="validator">The validator to add</param>
        /// <returns>A new UnionValidator with the additional validator</returns>
        public UnionValidator Or<T>(IValidator<T> validator)
        {
            return Or(new TypedValidatorWrapper<T>(validator));
        }

        /// <summary>
        /// Gets the types that this union validator can accept.
        /// </summary>
        /// <returns>An array of accepted types</returns>
        public Type[] GetAcceptedTypes()
        {
            return _validators.Select(v => v.ValidatedType).ToArray();
        }

        /// <summary>
        /// Wrapper class to adapt typed validators to the non-generic IValidator interface.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        private sealed class TypedValidatorWrapper<T> : IValidator
        {
            private readonly IValidator<T> _validator;

            public TypedValidatorWrapper(IValidator<T> validator)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            }

            public Type ValidatedType => typeof(T);

            public ValidationResult Validate(object? value)
            {
                if (value is T typedValue)
                    return _validator.Validate(typedValue);

                if (value is null && !typeof(T).IsValueType)
                    return _validator.Validate(default(T)!);

                return ValidationResult.Failure($"Expected value of type {typeof(T).Name}, but got {value?.GetType().Name ?? "null"}.");
            }
        }
    }

    /// <summary>
    /// Generic union validator for two specific types.
    /// </summary>
    /// <typeparam name="T1">The first possible type</typeparam>
    /// <typeparam name="T2">The second possible type</typeparam>
    public sealed class UnionValidator<T1, T2> : IValidator<object?>, IValidator
    {
        private readonly IValidator<T1> _validator1;
        private readonly IValidator<T2> _validator2;

        /// <summary>
        /// Gets the type of value this validator can validate.
        /// </summary>
        public Type ValidatedType => typeof(object);

        /// <summary>
        /// Initializes a new instance of the UnionValidator class.
        /// </summary>
        /// <param name="validator1">The validator for the first type</param>
        /// <param name="validator2">The validator for the second type</param>
        public UnionValidator(IValidator<T1> validator1, IValidator<T2> validator2)
        {
            _validator1 = validator1 ?? throw new ArgumentNullException(nameof(validator1));
            _validator2 = validator2 ?? throw new ArgumentNullException(nameof(validator2));
        }

        /// <summary>
        /// Validates the value as either T1 or T2.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result</returns>
        public ValidationResult Validate(object? value)
        {
            // Try T1 first
            if (value is T1 value1 || (value is null && !typeof(T1).IsValueType))
            {
                var result1 = _validator1.Validate(value is T1 ? (T1)value : default(T1)!);
                if (result1.IsValid)
                    return ValidationResult.Success();
            }

            // Try T2 second
            if (value is T2 value2 || (value is null && !typeof(T2).IsValueType))
            {
                var result2 = _validator2.Validate(value is T2 ? (T2)value : default(T2)!);
                if (result2.IsValid)
                    return ValidationResult.Success();
            }

            return ValidationResult.Failure($"Value must be either {typeof(T1).Name} or {typeof(T2).Name}, but got {value?.GetType().Name ?? "null"}.");
        }

        /// <summary>
        /// Validates the specified value and throws an exception if validation fails.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>The validated value</returns>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public object? ValidateAndThrow(object? value)
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
        public bool TryValidate(object? value, out ValidationResult result)
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
    }
} 