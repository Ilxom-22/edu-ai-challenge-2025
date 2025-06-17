# ValidationLibrary

A robust, type-safe validation library for .NET 8+ with **100% test coverage** and **comprehensive error reporting**. This library provides a fluent API for validating primitive and complex data structures with detailed error messages and property paths.

## Features

- **Type-safe validation** with compile-time checking
- **Fluent API** for easy and readable validation rules
- **Comprehensive error reporting** with detailed error messages and property paths
- **Extensible architecture** for custom validators
- **Support for primitive types** (string, numeric, boolean, DateTime, enum)
- **Complex type validation** for objects and collections
- **Conditional validation** with When/Unless patterns
- **Union types** for validating one of multiple possible types
- **Nullable and optional value support**
- **Custom validation functions**
- **Exception handling** with ValidationException
- **Thread-safe** validators that can be reused across multiple threads

## Installation

Since this library is not published as a NuGet package, add it to your project as a project reference:

```xml
<ProjectReference Include="path\to\ValidationLibrary\ValidationLibrary.csproj" />
```

## Quick Start

```csharp
using ValidationLibrary;

// Simple string validation
var nameValidator = Validator.String()
    .NotEmpty()
    .MinLength(2)
    .MaxLength(50);

var result = nameValidator.Validate("John Doe");
if (result.IsValid)
{
    Console.WriteLine("Valid name!");
}
else
{
    Console.WriteLine($"Invalid: {result.ErrorMessage}");
    // Output: "Value cannot be null or empty." (if empty)
    // Output: "Value must be at least 2 characters long." (if too short)
}
```

## Validator Types

### String Validation

```csharp
var stringValidator = Validator.String()
    .NotNull("Value cannot be null.")
    .NotEmpty("Value cannot be null or empty.")
    .NotWhiteSpace("Value cannot be null, empty, or whitespace.")
    .MinLength(5, "Value must be at least 5 characters long.")
    .MaxLength(100, "Value must be at most 100 characters long.")
    .Length(10) // Exact length: "Value must be exactly 10 characters long."
    .Length(5, 15) // Length range: "Value must be between 5 and 15 characters long."
    .Matches(@"^[a-zA-Z]+$", "Value must match the pattern: ^[a-zA-Z]+$")
    .Contains("test", "Value must contain: test")
    .StartsWith("Hello", "Value must start with: Hello")
    .EndsWith("World", "Value must end with: World")
    .Email("Value must be a valid email address.")
    .Url("Value must be a valid URL.")
    .Guid("Value must be a valid GUID.")
    .OneOf(new[] { "option1", "option2" }, "Value must be one of: option1, option2");
```

### Numeric Validation

```csharp
var intValidator = Validator.Int()
    .GreaterThan(0, "Value must be greater than 0.")
    .GreaterThanOrEqual(1, "Value must be greater than or equal to 1.")
    .LessThan(100, "Value must be less than 100.")
    .LessThanOrEqual(99, "Value must be less than or equal to 99.")
    .Between(1, 99, "Value must be between 1 and 99.")
    .Equal(42, "Value must be equal to 42.")
    .NotEqual(0, "Value must not be equal to 0.")
    .Positive("Value must be positive.")
    .Negative("Value must be negative.")
    .Zero("Value must be zero.")
    .NotZero("Value must not be zero.")
    .OneOf(new[] { 1, 2, 3, 5, 8 }, "Value must be one of: 1, 2, 3, 5, 8");

// Other numeric types
var doubleValidator = Validator.Double().Between(0.0, 100.0);
var decimalValidator = Validator.Decimal().Positive();
var longValidator = Validator.Long().GreaterThan(1000000000L);
var floatValidator = Validator.Float().Between(1.0f, 10.0f);
var shortValidator = Validator.Short().Between((short)1, (short)100);
var byteValidator = Validator.Byte().LessThan((byte)200);
```

### Boolean Validation

```csharp
var boolValidator = Validator.Boolean()
    .IsTrue("Value must be true.")
    .IsFalse("Value must be false.")
    .Equal(true, "Value must be equal to True.");
```

### DateTime Validation

```csharp
var dateValidator = Validator.DateTime()
    .After(DateTime.Now)
    .Before(DateTime.Now.AddYears(1))
    .Between(DateTime.Now, DateTime.Now.AddMonths(1))
    .InThePast()
    .InTheFuture()
    .IsToday()
    .NotDefault()
    .WithinDaysFromToday(30)
    .HasKind(DateTimeKind.Utc);
```

### Enum Validation

```csharp
public enum Status { Active, Inactive, Pending }

var enumValidator = Validator.Enum<Status>()
    .IsDefined()
    .OneOf(new[] { Status.Active, Status.Pending })
    .NotEqual(Status.Inactive)
    .Equal(Status.Active);
```

### Collection Validation

```csharp
var arrayValidator = Validator.Array<string>()
    .NotNull("Collection cannot be null.")
    .NotEmpty("Collection cannot be empty.")
    .MinCount(1, "Collection must contain at least 1 items.")
    .MaxCount(10, "Collection must contain at most 10 items.")
    .Count(5, "Collection must contain exactly 5 items.")
    .Count(3, 7, "Collection must contain between 3 and 7 items.")
    .Unique("Collection must contain unique items.")
    .Contains("required-item", "Collection must contain: required-item")
    .DoesNotContain("forbidden-item", "Collection must not contain: forbidden-item")
    .ForEach(Validator.String().NotEmpty()) // Validate each item
    .All(item => item.StartsWith("prefix"), "All items in the collection must satisfy the condition.")
    .Any(item => item.Contains("keyword"), "At least one item in the collection must satisfy the condition.");

// Other collection types
var listValidator = Validator.List<int>();
var enumerableValidator = Validator.Enumerable<double>();
var collectionValidator = Validator.Collection<string>();
```

### Object Validation

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string[] Tags { get; set; }
    public bool IsActive { get; set; }
}

var userValidator = Validator.Object<User>()
    .NotNull("User cannot be null.")
    .RuleFor(u => u.Name, Validator.String().NotEmpty().MinLength(2))
    .RuleFor(u => u.Email, Validator.String().Email())
    .RuleFor(u => u.Age, Validator.Int().Between(0, 150))
    .RuleFor(u => u.DateOfBirth, Validator.DateTime().InThePast())
    .RuleFor(u => u.Tags, Validator.Array<string>().MaxCount(5)
        .ForEach(Validator.String().NotEmpty().MinLength(2)))
    .Must(u => u.Age >= 18, "User must be at least 18 years old");

// Validation result with property paths
var user = new User { Name = "J", Email = "invalid", Age = 16, Tags = new[] { "x", "" } };
var result = userValidator.Validate(user);

// Example errors with property paths:
// - "Name: Value must be at least 2 characters long."
// - "Email: Value must be a valid email address."
// - "Tags.[0]: Value must be at least 2 characters long."
// - "Tags.[1]: Value cannot be null or empty."
// - "User must be at least 18 years old"
```

### Conditional Validation

```csharp
var validator = Validator.Object<User>()
    .RuleFor(u => u.Name, Validator.String().NotEmpty())
    .When(u => u.Age >= 18, v => v
        .RuleFor(u => u.Email, Validator.String().Email()))
    .Unless(u => u.IsActive, v => v
        .RuleFor(u => u.Password, Validator.String().MinLength(8)));
```

### Custom Validation

```csharp
// Custom validator with function
var customValidator = Validator.Custom<string>(value =>
{
    if (string.IsNullOrEmpty(value))
        return ValidationResult.Failure("Value cannot be empty");
    
    if (value.Length % 2 != 0)
        return ValidationResult.Failure("Value length must be even");
    
    return ValidationResult.Success();
});

// Custom validator with predicate
var evenLengthValidator = Validator.Custom<string>(
    value => value?.Length % 2 == 0,
    "String length must be even");
```

### Nullable and Optional Values

```csharp
// Nullable value types
var nullableIntValidator = Validator.Nullable(Validator.Int().Positive());

// Optional reference types  
var optionalStringValidator = Validator.Optional(Validator.String().NotEmpty());
```

### Union Types

```csharp
// General union validator
var unionValidator = Validator.Union(
    Validator.String(),
    Validator.Int(),
    Validator.Boolean()
);
```

## Error Handling

### Basic Error Handling

```csharp
var validator = Validator.String().NotEmpty().MinLength(5);

// Check validation result
var result = validator.Validate("Hi");
if (!result.IsValid)
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
    // Output: "Value cannot be null or empty." (if empty)
    // Output: "Value must be at least 5 characters long." (if too short)
    
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"- {error.Message} (Code: {error.ErrorCode})");
        Console.WriteLine($"  Property: {error.PropertyPath}");
    }
}
```

### Exception Handling

```csharp
try
{
    var validatedValue = validator.ValidateAndThrow("Hi");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"- {error.PropertyPath}: {error.Message}");
    }
}
```

### Try-Validate Pattern

```csharp
if (validator.TryValidate("Hello World", out var result))
{
    Console.WriteLine("Validation succeeded");
}
else
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
}
```

## Validation Results and Errors

The `ValidationResult` class provides detailed information about validation outcomes:

```csharp
public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    public string? ErrorMessage { get; } // First error message
    
    // Static factory methods
    public static ValidationResult Success();
    public static ValidationResult Failure(string message);
    public static ValidationResult Failure(IEnumerable<ValidationError> errors);
    public static ValidationResult Combine(params ValidationResult[] results);
}

public class ValidationError
{
    public string? Message { get; }        // Supports null messages
    public string? PropertyName { get; }
    public string? PropertyPath { get; }   // Full path for nested properties
    public string? ErrorCode { get; }      // Programmatic error identification
    public object? Context { get; }        // Additional error context
    
    // Create copies with updated properties
    public ValidationError WithPath(string newPath);
    public ValidationError WithCode(string errorCode);
    public ValidationError WithContext(object context);
}
```

## Extension Methods

The library includes extension methods for enhanced functionality:

```csharp
// Transform values before validation
var transformValidator = Validator.String()
    .Transform<int>(int.Parse)
    .Positive();

// Custom error messages
var customValidator = validator.WithMessage((value, result) => 
    $"Custom error for {value}");
```

## Architecture

### Core Components

- **`IValidator<T>`**: Base interface for all validators
- **`BaseValidator<T>`**: Abstract base class providing common functionality
- **`ValidationResult`**: Immutable result containing success status and errors
- **`ValidationError`**: Immutable error with detailed information (supports null messages)
- **`ValidationException`**: Exception thrown by `ValidateAndThrow`

### Validator Types

- **`StringValidator`**: String-specific validation rules with smart error handling
- **`NumericValidator<T>`**: Generic numeric validation for all numeric types
- **`BooleanValidator`**: Boolean validation rules
- **`DateTimeValidator`**: DateTime-specific validation
- **`EnumValidator<T>`**: Enum validation with type safety
- **`CollectionValidator<TCollection, TItem>`**: Generic collection validation
- **`ObjectValidator<T>`**: Complex object validation with property rules and paths
- **`CustomValidator<T>`**: Custom validation functions
- **`NullableValidator<T>`**: Nullable value type validation
- **`OptionalValidator<T>`**: Nullable reference type validation
- **`UnionValidator`**: One-of-multiple-types validation

### Reliability Features
- **Null-safe**: Proper handling of null and empty values
- **Smart error reporting**: Avoids redundant error messages (e.g., empty strings don't trigger both NotEmpty and MinLength errors)
- **Consistent error messages**: Standardized error message formats across all validators
- **Property path tracking**: Detailed error location information for nested objects

## Best Practices

1. **Reuse validators**: Create validator instances once and reuse them for better performance
2. **Use meaningful error messages**: Provide clear, actionable error messages for users
3. **Validate early**: Validate input as soon as possible in your application flow
4. **Use property paths**: For complex objects, property paths help identify exact error locations
5. **Handle exceptions gracefully**: Use try-catch or TryValidate for robust error handling
6. **Combine validators wisely**: Use conditional and combined validators for complex business rules
7. **Leverage error codes**: Use error codes for programmatic error handling and localization

## Performance Considerations

- **Lightweight validators**: Validators have minimal overhead and can be cached/reused
- **Compiled expressions**: Property expressions are compiled once and cached for optimal performance
- **Regex optimization**: Regular expressions are compiled for better repeated use performance
- **Exception-free paths**: Use `TryValidate` to avoid exceptions in performance-critical paths
- **Smart validation**: MinLength skips validation for null/empty strings to prevent redundant checks

## Thread Safety

All validators are **thread-safe** and can be used concurrently across multiple threads. Validator instances are immutable after creation, making them safe for concurrent use.

## Error Message Examples

### Common Error Messages
- **String**: `"Value cannot be null or empty."`, `"Value must be at least 5 characters long."`
- **Numeric**: `"Value must be equal to 42."`, `"Value must not be equal to 0."`
- **Boolean**: `"Value must be equal to True."`, `"Value must be false."`
- **Collection**: `"Collection must contain: required-item"`, `"Collection must contain unique items."`
- **DateTime**: `"Value must be in the past."`, `"Value must be after {date}."`

### Property Path Examples
For nested object validation:
```
- "Name: Value must be at least 2 characters long."
- "Address.Street: Value cannot be null or empty."
- "Tags.[0]: Value must be at least 2 characters long."
- "Users.[1].Email: Value must be a valid email address."
```

## License

This library is released under the MIT License. 