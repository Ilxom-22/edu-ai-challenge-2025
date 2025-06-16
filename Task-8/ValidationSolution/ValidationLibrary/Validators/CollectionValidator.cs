using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for collection types with fluent API support.
    /// </summary>
    /// <typeparam name="TCollection">The collection type</typeparam>
    /// <typeparam name="TItem">The item type within the collection</typeparam>
    public sealed class CollectionValidator<TCollection, TItem> : BaseValidator<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the CollectionValidator class.
        /// </summary>
        public CollectionValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new CollectionValidator instance</returns>
        protected override BaseValidator<TCollection> CreateInstance() => new CollectionValidator<TCollection, TItem>();

        /// <summary>
        /// Validates that the collection is not null.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> NotNull(string message = "Collection cannot be null.")
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null,
                message,
                "COLLECTION_NOT_NULL");
        }

        /// <summary>
        /// Validates that the collection is not empty.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> NotEmpty(string message = "Collection cannot be empty.")
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null && value.Any(),
                message,
                "COLLECTION_NOT_EMPTY");
        }

        /// <summary>
        /// Validates that the collection has a minimum number of items.
        /// </summary>
        /// <param name="minCount">The minimum number of items</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> MinCount(int minCount, string? message = null)
        {
            message ??= $"Collection must contain at least {minCount} items.";
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null && value.Count() >= minCount,
                message,
                "COLLECTION_MIN_COUNT");
        }

        /// <summary>
        /// Validates that the collection has a maximum number of items.
        /// </summary>
        /// <param name="maxCount">The maximum number of items</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> MaxCount(int maxCount, string? message = null)
        {
            message ??= $"Collection must contain at most {maxCount} items.";
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null && value.Count() <= maxCount,
                message,
                "COLLECTION_MAX_COUNT");
        }

        /// <summary>
        /// Validates that the collection has an exact number of items.
        /// </summary>
        /// <param name="count">The exact number of items</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> Count(int count, string? message = null)
        {
            message ??= $"Collection must contain exactly {count} items.";
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null && value.Count() == count,
                message,
                "COLLECTION_EXACT_COUNT");
        }

        /// <summary>
        /// Validates that the collection count is within a specified range.
        /// </summary>
        /// <param name="minCount">The minimum number of items</param>
        /// <param name="maxCount">The maximum number of items</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> Count(int minCount, int maxCount, string? message = null)
        {
            message ??= $"Collection must contain between {minCount} and {maxCount} items.";
            return (CollectionValidator<TCollection, TItem>)AddRule(
                value => value != null && value.Count() >= minCount && value.Count() <= maxCount,
                message,
                "COLLECTION_COUNT_RANGE");
        }

        /// <summary>
        /// Validates each item in the collection using the specified validator.
        /// </summary>
        /// <param name="itemValidator">The validator to apply to each item</param>
        /// <param name="stopOnFirstError">Whether to stop validation on the first error</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> ForEach(IValidator<TItem> itemValidator, bool stopOnFirstError = false)
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Success();

                var errors = new List<ValidationError>();
                var index = 0;

                foreach (var item in value)
                {
                    var result = itemValidator.Validate(item);
                    if (!result.IsValid)
                    {
                        var indexedErrors = result.Errors
                            .Select(e => e.WithPath($"[{index}]" + (string.IsNullOrEmpty(e.PropertyPath) ? "" : "." + e.PropertyPath)))
                            .ToList();
                        errors.AddRange(indexedErrors);

                        if (stopOnFirstError)
                            break;
                    }
                    index++;
                }

                return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
            });
        }

        /// <summary>
        /// Validates that all items in the collection are unique.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <param name="comparer">Optional equality comparer</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> Unique(string message = "Collection items must be unique.", IEqualityComparer<TItem>? comparer = null)
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Success();

                var items = value.ToList();
                var uniqueItems = comparer != null ? items.Distinct(comparer).ToList() : items.Distinct().ToList();
                
                return items.Count == uniqueItems.Count
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message);
            });
        }

        /// <summary>
        /// Validates that the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item that must be present</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparer">Optional equality comparer</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> Contains(TItem item, string? message = null, IEqualityComparer<TItem>? comparer = null)
        {
            message ??= $"Collection must contain the item: {item}";
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Failure(message);

                var contains = comparer != null ? value.Contains(item, comparer) : value.Contains(item);
                return contains
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message);
            });
        }

        /// <summary>
        /// Validates that the collection does not contain a specific item.
        /// </summary>
        /// <param name="item">The item that must not be present</param>
        /// <param name="message">Custom error message</param>
        /// <param name="comparer">Optional equality comparer</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> DoesNotContain(TItem item, string? message = null, IEqualityComparer<TItem>? comparer = null)
        {
            message ??= $"Collection must not contain the item: {item}";
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Success();

                var contains = comparer != null ? value.Contains(item, comparer) : value.Contains(item);
                return !contains
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message);
            });
        }

        /// <summary>
        /// Validates that all items in the collection satisfy a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to test each item</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> All(Func<TItem, bool> predicate, string message = "All items in the collection must satisfy the condition.")
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Success();

                return value.All(predicate)
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message);
            });
        }

        /// <summary>
        /// Validates that at least one item in the collection satisfies a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to test each item</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public CollectionValidator<TCollection, TItem> Any(Func<TItem, bool> predicate, string message = "At least one item in the collection must satisfy the condition.")
        {
            return (CollectionValidator<TCollection, TItem>)AddRule(value =>
            {
                if (value == null)
                    return ValidationResult.Failure(message, "COLLECTION_ANY");

                return value.Any(predicate)
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(message, "COLLECTION_ANY");
            });
        }
    }

    /// <summary>
    /// Provides static factory methods for creating collection validators.
    /// </summary>
    public static class CollectionValidator
    {
        /// <summary>
        /// Creates a new validator for arrays.
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <returns>A new CollectionValidator for arrays</returns>
        public static CollectionValidator<T[], T> Array<T>() => new();

        /// <summary>
        /// Creates a new validator for lists.
        /// </summary>
        /// <typeparam name="T">The list item type</typeparam>
        /// <returns>A new CollectionValidator for lists</returns>
        public static CollectionValidator<List<T>, T> List<T>() => new();

        /// <summary>
        /// Creates a new validator for IEnumerable collections.
        /// </summary>
        /// <typeparam name="T">The collection item type</typeparam>
        /// <returns>A new CollectionValidator for IEnumerable</returns>
        public static CollectionValidator<IEnumerable<T>, T> Enumerable<T>() => new();

        /// <summary>
        /// Creates a new validator for ICollection collections.
        /// </summary>
        /// <typeparam name="T">The collection item type</typeparam>
        /// <returns>A new CollectionValidator for ICollection</returns>
        public static CollectionValidator<ICollection<T>, T> Collection<T>() => new();
    }
} 