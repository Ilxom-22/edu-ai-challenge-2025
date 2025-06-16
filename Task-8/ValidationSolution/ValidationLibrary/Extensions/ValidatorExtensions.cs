using ValidationLibrary.Core;

namespace ValidationLibrary.Extensions
{
    /// <summary>
    /// Extension methods to enhance the validation library's functionality and extensibility.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Applies a transformation to the value before validation.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <typeparam name="TOutput">The output type after transformation</typeparam>
        /// <param name="validator">The validator for the transformed value</param>
        /// <param name="transform">The transformation function</param>
        /// <returns>A new validator that applies the transformation</returns>
        public static IValidator<TInput> Transform<TInput, TOutput>(
            this IValidator<TOutput> validator,
            Func<TInput, TOutput> transform)
        {
            return new TransformValidator<TInput, TOutput>(validator, transform);
        }

        /// <summary>
        /// Adds a custom error message override for validation failures.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validator">The validator</param>
        /// <param name="errorMessageProvider">Function to provide custom error message</param>
        /// <returns>A new validator with custom error messaging</returns>
        public static IValidator<T> WithMessage<T>(
            this IValidator<T> validator,
            Func<T, ValidationResult, string> errorMessageProvider)
        {
            return new CustomMessageValidator<T>(validator, errorMessageProvider);
        }

        /// <summary>
        /// Adds a simple custom error message override for validation failures.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validator">The validator</param>
        /// <param name="errorMessage">The custom error message</param>
        /// <returns>A new validator with custom error messaging</returns>
        public static IValidator<T> WithMessage<T>(
            this IValidator<T> validator,
            string errorMessage)
        {
            return validator.WithMessage((_, _) => errorMessage);
        }

        /// <summary>
        /// Validates a collection and returns detailed results for each item.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="validator">The validator for each item</param>
        /// <param name="collection">The collection to validate</param>
        /// <returns>A dictionary mapping indices to validation results</returns>
        public static Dictionary<int, ValidationResult> ValidateAll<T>(
            this IValidator<T> validator,
            IEnumerable<T> collection)
        {
            var results = new Dictionary<int, ValidationResult>();
            var index = 0;

            foreach (var item in collection)
            {
                results[index] = validator.Validate(item);
                index++;
            }

            return results;
        }

        /// <summary>
        /// Validates a collection and returns only the failed items with their indices.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="validator">The validator for each item</param>
        /// <param name="collection">The collection to validate</param>
        /// <returns>A dictionary mapping indices to validation results for failed items only</returns>
        public static Dictionary<int, ValidationResult> ValidateFailures<T>(
            this IValidator<T> validator,
            IEnumerable<T> collection)
        {
            return validator.ValidateAll(collection)
                           .Where(kvp => !kvp.Value.IsValid)
                           .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Creates a validator that applies multiple validators and requires all to pass.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validators">The validators to combine</param>
        /// <returns>A combined validator</returns>
        public static IValidator<T> All<T>(params IValidator<T>[] validators)
        {
            return new CombinedValidator<T>(validators, CombineMode.All);
        }

        /// <summary>
        /// Creates a validator that applies multiple validators and requires at least one to pass.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validators">The validators to combine</param>
        /// <returns>A combined validator</returns>
        public static IValidator<T> Any<T>(params IValidator<T>[] validators)
        {
            return new CombinedValidator<T>(validators, CombineMode.Any);
        }

        /// <summary>
        /// Adds validation that only applies when a condition is met.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validator">The base validator</param>
        /// <param name="condition">The condition that must be true</param>
        /// <param name="conditionalValidator">The validator to apply when condition is true</param>
        /// <returns>A new conditional validator</returns>
        public static IValidator<T> When<T>(
            this IValidator<T> validator,
            Func<T, bool> condition,
            IValidator<T> conditionalValidator)
        {
            return new ConditionalValidator<T>(validator, condition, conditionalValidator);
        }

        /// <summary>
        /// Adds validation that only applies when a condition is NOT met.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validator">The base validator</param>
        /// <param name="condition">The condition that must be false</param>
        /// <param name="conditionalValidator">The validator to apply when condition is false</param>
        /// <returns>A new conditional validator</returns>
        public static IValidator<T> Unless<T>(
            this IValidator<T> validator,
            Func<T, bool> condition,
            IValidator<T> conditionalValidator)
        {
            return validator.When(x => !condition(x), conditionalValidator);
        }

        // Supporting classes for extensions

        private sealed class TransformValidator<TInput, TOutput> : IValidator<TInput>
        {
            private readonly IValidator<TOutput> _validator;
            private readonly Func<TInput, TOutput> _transform;

            public TransformValidator(IValidator<TOutput> validator, Func<TInput, TOutput> transform)
            {
                _validator = validator;
                _transform = transform;
            }

            public ValidationResult Validate(TInput value)
            {
                try
                {
                    var transformed = _transform(value);
                    return _validator.Validate(transformed);
                }
                catch (Exception ex)
                {
                    return ValidationResult.Failure($"Transformation failed: {ex.Message}");
                }
            }

            public TInput ValidateAndThrow(TInput value)
            {
                var result = Validate(value);
                if (!result.IsValid)
                    throw new ValidationException(result);
                return value;
            }

            public bool TryValidate(TInput value, out ValidationResult result)
            {
                result = Validate(value);
                return result.IsValid;
            }
        }

        private sealed class CustomMessageValidator<T> : IValidator<T>
        {
            private readonly IValidator<T> _validator;
            private readonly Func<T, ValidationResult, string> _errorMessageProvider;

            public CustomMessageValidator(IValidator<T> validator, Func<T, ValidationResult, string> errorMessageProvider)
            {
                _validator = validator;
                _errorMessageProvider = errorMessageProvider;
            }

            public ValidationResult Validate(T value)
            {
                var result = _validator.Validate(value);
                if (result.IsValid)
                    return result;

                var customMessage = _errorMessageProvider(value, result);
                return ValidationResult.Failure(customMessage);
            }

            public T ValidateAndThrow(T value)
            {
                var result = Validate(value);
                if (!result.IsValid)
                    throw new ValidationException(result);
                return value;
            }

            public bool TryValidate(T value, out ValidationResult result)
            {
                result = Validate(value);
                return result.IsValid;
            }
        }

        private enum CombineMode
        {
            All,
            Any
        }

        private sealed class CombinedValidator<T> : IValidator<T>
        {
            private readonly IValidator<T>[] _validators;
            private readonly CombineMode _mode;

            public CombinedValidator(IValidator<T>[] validators, CombineMode mode)
            {
                _validators = validators;
                _mode = mode;
            }

            public ValidationResult Validate(T value)
            {
                var results = _validators.Select(v => v.Validate(value)).ToList();

                return _mode switch
                {
                    CombineMode.All => ValidationResult.Combine(results.ToArray()),
                    CombineMode.Any => results.Any(r => r.IsValid) 
                        ? ValidationResult.Success() 
                        : ValidationResult.Combine(results.ToArray()),
                    _ => throw new InvalidOperationException($"Unknown combine mode: {_mode}")
                };
            }

            public T ValidateAndThrow(T value)
            {
                var result = Validate(value);
                if (!result.IsValid)
                    throw new ValidationException(result);
                return value;
            }

            public bool TryValidate(T value, out ValidationResult result)
            {
                result = Validate(value);
                return result.IsValid;
            }
        }

        private sealed class ConditionalValidator<T> : IValidator<T>
        {
            private readonly IValidator<T> _baseValidator;
            private readonly Func<T, bool> _condition;
            private readonly IValidator<T> _conditionalValidator;

            public ConditionalValidator(IValidator<T> baseValidator, Func<T, bool> condition, IValidator<T> conditionalValidator)
            {
                _baseValidator = baseValidator;
                _condition = condition;
                _conditionalValidator = conditionalValidator;
            }

            public ValidationResult Validate(T value)
            {
                var baseResult = _baseValidator.Validate(value);
                if (!baseResult.IsValid)
                    return baseResult;

                if (_condition(value))
                {
                    var conditionalResult = _conditionalValidator.Validate(value);
                    return baseResult.And(conditionalResult);
                }

                return baseResult;
            }

            public T ValidateAndThrow(T value)
            {
                var result = Validate(value);
                if (!result.IsValid)
                    throw new ValidationException(result);
                return value;
            }

            public bool TryValidate(T value, out ValidationResult result)
            {
                result = Validate(value);
                return result.IsValid;
            }
        }
    }
} 