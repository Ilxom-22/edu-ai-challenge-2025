using ValidationLibrary.Core;
using ValidationLibrary.Validators;

namespace ValidationLibrary
{
    /// <summary>
    /// Main entry point for the validation library. Provides static factory methods
    /// for creating validators with a fluent API similar to Zod or FluentValidation.
    /// </summary>
    public static class Validator
    {
        // String validators
        /// <summary>
        /// Creates a new string validator.
        /// </summary>
        /// <returns>A new StringValidator instance</returns>
        public static StringValidator String() => new();

        // Numeric validators
        /// <summary>
        /// Creates a new integer validator.
        /// </summary>
        /// <returns>A new NumericValidator for integers</returns>
        public static NumericValidator<int> Int() => NumericValidator.Int();

        /// <summary>
        /// Creates a new long validator.
        /// </summary>
        /// <returns>A new NumericValidator for longs</returns>
        public static NumericValidator<long> Long() => NumericValidator.Long();

        /// <summary>
        /// Creates a new float validator.
        /// </summary>
        /// <returns>A new NumericValidator for floats</returns>
        public static NumericValidator<float> Float() => NumericValidator.Float();

        /// <summary>
        /// Creates a new double validator.
        /// </summary>
        /// <returns>A new NumericValidator for doubles</returns>
        public static NumericValidator<double> Double() => NumericValidator.Double();

        /// <summary>
        /// Creates a new decimal validator.
        /// </summary>
        /// <returns>A new NumericValidator for decimals</returns>
        public static NumericValidator<decimal> Decimal() => NumericValidator.Decimal();

        /// <summary>
        /// Creates a new short validator.
        /// </summary>
        /// <returns>A new NumericValidator for shorts</returns>
        public static NumericValidator<short> Short() => NumericValidator.Short();

        /// <summary>
        /// Creates a new byte validator.
        /// </summary>
        /// <returns>A new NumericValidator for bytes</returns>
        public static NumericValidator<byte> Byte() => NumericValidator.Byte();

        // Collection validators
        /// <summary>
        /// Creates a new validator for arrays.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <returns>A new CollectionValidator for arrays</returns>
        public static CollectionValidator<T[], T> Array<T>() => CollectionValidator.Array<T>();

        /// <summary>
        /// Creates a new validator for lists.
        /// </summary>
        /// <typeparam name="T">The list item type</typeparam>
        /// <returns>A new CollectionValidator for lists</returns>
        public static CollectionValidator<List<T>, T> List<T>() => CollectionValidator.List<T>();

        /// <summary>
        /// Creates a new validator for IEnumerable collections.
        /// </summary>
        /// <typeparam name="T">The collection item type</typeparam>
        /// <returns>A new CollectionValidator for IEnumerable</returns>
        public static CollectionValidator<IEnumerable<T>, T> Enumerable<T>() => CollectionValidator.Enumerable<T>();

        /// <summary>
        /// Creates a new validator for ICollection collections.
        /// </summary>
        /// <typeparam name="T">The collection item type</typeparam>
        /// <returns>A new CollectionValidator for ICollection</returns>
        public static CollectionValidator<ICollection<T>, T> Collection<T>() => CollectionValidator.Collection<T>();

        // Object validators
        /// <summary>
        /// Creates a new validator for the specified object type.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns>A new ObjectValidator for the specified type</returns>
        public static ObjectValidator<T> Object<T>() where T : class => ObjectValidator.For<T>();

        // Boolean validator
        /// <summary>
        /// Creates a new boolean validator.
        /// </summary>
        /// <returns>A new BooleanValidator instance</returns>
        public static BooleanValidator Boolean() => new();

        // DateTime validator
        /// <summary>
        /// Creates a new DateTime validator.
        /// </summary>
        /// <returns>A new DateTimeValidator instance</returns>
        public static DateTimeValidator DateTime() => new();

        // Custom validator
        /// <summary>
        /// Creates a custom validator with the specified validation function.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="validationFunction">The validation function</param>
        /// <returns>A new custom validator</returns>
        public static CustomValidator<T> Custom<T>(Func<T, ValidationResult> validationFunction) => new(validationFunction);

        /// <summary>
        /// Creates a custom validator with the specified predicate and error message.
        /// </summary>
        /// <typeparam name="T">The type being validated</typeparam>
        /// <param name="predicate">The validation predicate</param>
        /// <param name="errorMessage">The error message if validation fails</param>
        /// <param name="errorCode">Optional error code</param>
        /// <returns>A new custom validator</returns>
        public static CustomValidator<T> Custom<T>(Func<T, bool> predicate, string errorMessage, string? errorCode = null)
            => new(value => predicate(value) 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(errorMessage));

        // Nullable validators
        /// <summary>
        /// Creates a validator for nullable values.
        /// </summary>
        /// <typeparam name="T">The underlying value type</typeparam>
        /// <param name="valueValidator">The validator for the non-null value</param>
        /// <returns>A new nullable validator</returns>
        public static NullableValidator<T> Nullable<T>(IValidator<T> valueValidator) where T : struct
            => new(valueValidator);

        /// <summary>
        /// Creates a validator for optional (nullable reference) values.
        /// </summary>
        /// <typeparam name="T">The reference type</typeparam>
        /// <param name="valueValidator">The validator for the non-null value</param>
        /// <returns>A new optional validator</returns>
        public static OptionalValidator<T> Optional<T>(IValidator<T> valueValidator) where T : class
            => new(valueValidator);

        // Enum validator
        /// <summary>
        /// Creates a new enum validator.
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <returns>A new EnumValidator instance</returns>
        public static EnumValidator<TEnum> Enum<TEnum>() where TEnum : struct, Enum => new();

        // Union/OneOf validator
        /// <summary>
        /// Creates a validator that accepts one of multiple possible types.
        /// </summary>
        /// <param name="validators">The validators for each possible type</param>
        /// <returns>A new union validator</returns>
        public static UnionValidator Union(params IValidator[] validators) => new(validators);

        /// <summary>
        /// Combines multiple validation results into a single result.
        /// </summary>
        /// <param name="results">The validation results to combine</param>
        /// <returns>A combined validation result</returns>
        public static ValidationResult Combine(params ValidationResult[] results)
            => ValidationResult.Combine(results);
    }
} 