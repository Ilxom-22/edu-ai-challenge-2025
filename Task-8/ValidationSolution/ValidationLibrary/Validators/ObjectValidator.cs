using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for complex object types with property-level validation support.
    /// </summary>
    /// <typeparam name="T">The object type being validated</typeparam>
    public sealed class ObjectValidator<T> : BaseValidator<T> where T : class
    {
        private readonly Dictionary<string, Func<T, ValidationResult>> _propertyValidators = new();

        /// <summary>
        /// Initializes a new instance of the ObjectValidator class.
        /// </summary>
        public ObjectValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new ObjectValidator instance</returns>
        protected override BaseValidator<T> CreateInstance()
        {
            var clone = new ObjectValidator<T>();
            foreach (var kvp in _propertyValidators)
            {
                clone._propertyValidators[kvp.Key] = kvp.Value;
            }
            return clone;
        }

        /// <summary>
        /// Validates that the object is not null.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> NotNull(string message = "Object cannot be null.")
        {
            return (ObjectValidator<T>)AddRule(value => value != null, message, "OBJECT_NOT_NULL");
        }

        /// <summary>
        /// Adds validation for a specific property.
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">Expression to select the property</param>
        /// <param name="validator">The validator for the property</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression, IValidator<TProperty> validator)
        {
            var propertyName = GetPropertyName(propertyExpression);
            var propertyGetter = propertyExpression.Compile();

            _propertyValidators[propertyName] = obj =>
            {
                if (obj == null)
                    return ValidationResult.Success();

                var propertyValue = propertyGetter(obj);
                var result = validator.Validate(propertyValue);

                if (result.IsValid)
                    return ValidationResult.Success();

                var errorsWithPropertyPath = result.Errors
                    .Select(e => e.WithPath(propertyName + (string.IsNullOrEmpty(e.PropertyPath) ? "" : "." + e.PropertyPath)))
                    .ToList();

                return ValidationResult.Failure(errorsWithPropertyPath);
            };

            return this;
        }

        /// <summary>
        /// Adds validation for a specific property with a custom validation function.
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">Expression to select the property</param>
        /// <param name="validationFunction">Custom validation function</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<TProperty, ValidationResult> validationFunction)
        {
            var propertyName = GetPropertyName(propertyExpression);
            var propertyGetter = propertyExpression.Compile();

            _propertyValidators[propertyName] = obj =>
            {
                if (obj == null)
                    return ValidationResult.Success();

                var propertyValue = propertyGetter(obj);
                var result = validationFunction(propertyValue);

                if (result.IsValid)
                    return ValidationResult.Success();

                var errorsWithPropertyPath = result.Errors
                    .Select(e => e.WithPath(propertyName + (string.IsNullOrEmpty(e.PropertyPath) ? "" : "." + e.PropertyPath)))
                    .ToList();

                return ValidationResult.Failure(errorsWithPropertyPath);
            };

            return this;
        }

        /// <summary>
        /// Adds validation for a specific property with a predicate and error message.
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">Expression to select the property</param>
        /// <param name="predicate">Predicate to validate the property</param>
        /// <param name="message">Error message if validation fails</param>
        /// <param name="errorCode">Optional error code</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<TProperty, bool> predicate, string message, string? errorCode = null)
        {
            var propertyName = GetPropertyName(propertyExpression);
            var propertyGetter = propertyExpression.Compile();

            _propertyValidators[propertyName] = obj =>
            {
                if (obj == null)
                    return ValidationResult.Success();

                var propertyValue = propertyGetter(obj);
                if (predicate(propertyValue))
                    return ValidationResult.Success();

                var error = new ValidationError(message, propertyName, propertyName, errorCode);
                return ValidationResult.Failure(new[] { error });
            };

            return this;
        }

        /// <summary>
        /// Adds validation that depends on the entire object context.
        /// </summary>
        /// <param name="predicate">Predicate to validate the object</param>
        /// <param name="message">Error message if validation fails</param>
        /// <param name="errorCode">Optional error code</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> Must(Func<T, bool> predicate, string message, string? errorCode = null)
        {
            return (ObjectValidator<T>)AddRule(obj =>
            {
                if (obj == null)
                    return ValidationResult.Success();

                return predicate(obj)
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message);
            });
        }

        /// <summary>
        /// Adds conditional validation based on a predicate.
        /// </summary>
        /// <param name="predicate">Condition that must be true for validation to apply</param>
        /// <param name="validator">Validator to apply when condition is true</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> When(Func<T, bool> predicate, Func<ObjectValidator<T>, ObjectValidator<T>> validator)
        {
            var conditionalValidator = validator(new ObjectValidator<T>());
            
            return (ObjectValidator<T>)AddRule(obj =>
            {
                if (obj == null || !predicate(obj))
                    return ValidationResult.Success();

                return conditionalValidator.Validate(obj);
            });
        }

        /// <summary>
        /// Adds conditional validation based on a predicate (opposite of When).
        /// </summary>
        /// <param name="predicate">Condition that must be false for validation to apply</param>
        /// <param name="validator">Validator to apply when condition is false</param>
        /// <returns>This validator instance for method chaining</returns>
        public ObjectValidator<T> Unless(Func<T, bool> predicate, Func<ObjectValidator<T>, ObjectValidator<T>> validator)
        {
            return When(obj => !predicate(obj), validator);
        }

        /// <summary>
        /// Overrides the validation method to include property validators.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>A validation result</returns>
        public override ValidationResult Validate(T value)
        {
            var results = new List<ValidationResult>();

            // Run base validation first
            var baseResult = base.Validate(value);
            results.Add(baseResult);

            // Run property validators
            if (value != null)
            {
                foreach (var propertyValidator in _propertyValidators.Values)
                {
                    var result = propertyValidator(value);
                    results.Add(result);
                }
            }

            return ValidationResult.Combine(results.ToArray());
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression</param>
        /// <returns>The property name</returns>
        private static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            if (propertyExpression.Body is UnaryExpression { Operand: MemberExpression unaryMemberExpression })
                return unaryMemberExpression.Member.Name;

            throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
        }
    }

    /// <summary>
    /// Provides static factory methods for creating object validators.
    /// </summary>
    public static class ObjectValidator
    {
        /// <summary>
        /// Creates a new validator for the specified object type.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns>A new ObjectValidator for the specified type</returns>
        public static ObjectValidator<T> For<T>() where T : class => new();
    }
} 