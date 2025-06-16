using ValidationLibrary.Core;

namespace ValidationLibrary.Validators
{
    /// <summary>
    /// Validator for DateTime values with fluent API support.
    /// </summary>
    public sealed class DateTimeValidator : BaseValidator<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeValidator class.
        /// </summary>
        public DateTimeValidator()
        {
        }

        /// <summary>
        /// Creates a new instance of this validator type.
        /// </summary>
        /// <returns>A new DateTimeValidator instance</returns>
        protected override BaseValidator<DateTime> CreateInstance() => new DateTimeValidator();

        /// <summary>
        /// Validates that the DateTime is after the specified date.
        /// </summary>
        /// <param name="minDate">The minimum date (exclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator After(DateTime minDate, string? message = null)
        {
            message ??= $"Date must be after {minDate:yyyy-MM-dd HH:mm:ss}.";
            return (DateTimeValidator)AddRule(value => value > minDate, message, "DATETIME_AFTER");
        }

        /// <summary>
        /// Validates that the DateTime is on or after the specified date.
        /// </summary>
        /// <param name="minDate">The minimum date (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator OnOrAfter(DateTime minDate, string? message = null)
        {
            message ??= $"Date must be on or after {minDate:yyyy-MM-dd HH:mm:ss}.";
            return (DateTimeValidator)AddRule(value => value >= minDate, message, "DATETIME_ON_OR_AFTER");
        }

        /// <summary>
        /// Validates that the DateTime is before the specified date.
        /// </summary>
        /// <param name="maxDate">The maximum date (exclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator Before(DateTime maxDate, string? message = null)
        {
            message ??= $"Date must be before {maxDate:yyyy-MM-dd HH:mm:ss}.";
            return (DateTimeValidator)AddRule(value => value < maxDate, message, "DATETIME_BEFORE");
        }

        /// <summary>
        /// Validates that the DateTime is on or before the specified date.
        /// </summary>
        /// <param name="maxDate">The maximum date (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator OnOrBefore(DateTime maxDate, string? message = null)
        {
            message ??= $"Date must be on or before {maxDate:yyyy-MM-dd HH:mm:ss}.";
            return (DateTimeValidator)AddRule(value => value <= maxDate, message, "DATETIME_ON_OR_BEFORE");
        }

        /// <summary>
        /// Validates that the DateTime is within the specified range.
        /// </summary>
        /// <param name="minDate">The minimum date (inclusive)</param>
        /// <param name="maxDate">The maximum date (inclusive)</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator Between(DateTime minDate, DateTime maxDate, string? message = null)
        {
            message ??= $"Date must be between {minDate:yyyy-MM-dd HH:mm:ss} and {maxDate:yyyy-MM-dd HH:mm:ss}.";
            return (DateTimeValidator)AddRule(
                value => value >= minDate && value <= maxDate,
                message,
                "DATETIME_BETWEEN");
        }

        /// <summary>
        /// Validates that the DateTime is in the past.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator InThePast(string message = "Date must be in the past.")
        {
            return (DateTimeValidator)AddRule(value => value < DateTime.Now, message, "DATETIME_PAST");
        }

        /// <summary>
        /// Validates that the DateTime is in the future.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator InTheFuture(string message = "Date must be in the future.")
        {
            return (DateTimeValidator)AddRule(value => value > DateTime.Now, message, "DATETIME_FUTURE");
        }

        /// <summary>
        /// Validates that the DateTime is today.
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator IsToday(string message = "Date must be today.")
        {
            return (DateTimeValidator)AddRule(value => value.Date == DateTime.Today, message, "DATETIME_TODAY");
        }

        /// <summary>
        /// Validates that the DateTime is not the default value (DateTime.MinValue).
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator NotDefault(string message = "Date cannot be the default value.")
        {
            return (DateTimeValidator)AddRule(value => value != default, message, "DATETIME_NOT_DEFAULT");
        }

        /// <summary>
        /// Validates that the DateTime is within the specified number of days from today.
        /// </summary>
        /// <param name="days">The number of days from today</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator WithinDaysFromToday(int days, string? message = null)
        {
            message ??= $"Date must be within {days} days from today.";
            var today = DateTime.Today;
            var minDate = today.AddDays(-days);
            var maxDate = today.AddDays(days);
            
            return (DateTimeValidator)AddRule(
                value => value.Date >= minDate && value.Date <= maxDate,
                message,
                "DATETIME_WITHIN_DAYS");
        }

        /// <summary>
        /// Validates that the DateTime has the specified kind (Local, Utc, or Unspecified).
        /// </summary>
        /// <param name="kind">The required DateTimeKind</param>
        /// <param name="message">Custom error message</param>
        /// <returns>This validator instance for method chaining</returns>
        public DateTimeValidator HasKind(DateTimeKind kind, string? message = null)
        {
            message ??= $"Date must have DateTimeKind of {kind}.";
            return (DateTimeValidator)AddRule(value => value.Kind == kind, message, "DATETIME_KIND");
        }
    }
} 