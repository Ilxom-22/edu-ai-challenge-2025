namespace ValidationLibrary.Core
{
    /// <summary>
    /// Represents a single validation error with detailed information.
    /// </summary>
    public sealed class ValidationError : IEquatable<ValidationError>
    {
        /// <summary>
        /// Gets the error message describing what validation failed.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the name of the property that failed validation, if applicable.
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// Gets the path to the property that failed validation, useful for nested objects.
        /// </summary>
        public string? PropertyPath { get; }

        /// <summary>
        /// Gets the error code for programmatic error handling.
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Gets additional context or metadata about the error.
        /// </summary>
        public object? Context { get; }

        /// <summary>
        /// Initializes a new instance of the ValidationError class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="propertyName">The name of the property that failed validation</param>
        /// <param name="propertyPath">The path to the property that failed validation</param>
        /// <param name="errorCode">The error code for programmatic error handling</param>
        /// <param name="context">Additional context or metadata about the error</param>
        public ValidationError(string message, string? propertyName = null, string? propertyPath = null, string? errorCode = null, object? context = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            PropertyName = propertyName;
            PropertyPath = propertyPath ?? propertyName;
            ErrorCode = errorCode;
            Context = context;
        }

        /// <summary>
        /// Creates a new ValidationError with an updated property path.
        /// </summary>
        /// <param name="newPath">The new property path</param>
        /// <returns>A new ValidationError with the updated path</returns>
        public ValidationError WithPath(string newPath)
            => new(Message, PropertyName, newPath, ErrorCode, Context);

        /// <summary>
        /// Creates a new ValidationError with an error code.
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <returns>A new ValidationError with the error code</returns>
        public ValidationError WithCode(string errorCode)
            => new(Message, PropertyName, PropertyPath, errorCode, Context);

        /// <summary>
        /// Creates a new ValidationError with additional context.
        /// </summary>
        /// <param name="context">The additional context</param>
        /// <returns>A new ValidationError with the context</returns>
        public ValidationError WithContext(object context)
            => new(Message, PropertyName, PropertyPath, ErrorCode, context);

        /// <summary>
        /// Determines whether the specified ValidationError is equal to this instance.
        /// </summary>
        /// <param name="other">The ValidationError to compare with this instance</param>
        /// <returns>True if the specified ValidationError is equal to this instance; otherwise, false</returns>
        public bool Equals(ValidationError? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return Message == other.Message &&
                   PropertyName == other.PropertyName &&
                   PropertyPath == other.PropertyPath &&
                   ErrorCode == other.ErrorCode;
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with this instance</param>
        /// <returns>True if the specified object is equal to this instance; otherwise, false</returns>
        public override bool Equals(object? obj)
            => Equals(obj as ValidationError);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance</returns>
        public override int GetHashCode()
            => HashCode.Combine(Message, PropertyName, PropertyPath, ErrorCode);

        /// <summary>
        /// Returns a string representation of the validation error.
        /// </summary>
        /// <returns>A string representation of the validation error</returns>
        public override string ToString()
        {
            var result = Message;
            if (!string.IsNullOrEmpty(PropertyPath))
                result = $"{PropertyPath}: {result}";
            if (!string.IsNullOrEmpty(ErrorCode))
                result = $"[{ErrorCode}] {result}";
            return result;
        }

        /// <summary>
        /// Determines whether two ValidationError instances are equal.
        /// </summary>
        /// <param name="left">The first ValidationError to compare</param>
        /// <param name="right">The second ValidationError to compare</param>
        /// <returns>True if the ValidationError instances are equal; otherwise, false</returns>
        public static bool operator ==(ValidationError? left, ValidationError? right)
            => Equals(left, right);

        /// <summary>
        /// Determines whether two ValidationError instances are not equal.
        /// </summary>
        /// <param name="left">The first ValidationError to compare</param>
        /// <param name="right">The second ValidationError to compare</param>
        /// <returns>True if the ValidationError instances are not equal; otherwise, false</returns>
        public static bool operator !=(ValidationError? left, ValidationError? right)
            => !Equals(left, right);
    }
} 