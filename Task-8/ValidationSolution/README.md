# ValidationLibrary

A robust, type-safe validation library for .NET 8+. This library provides a fluent API for validating primitive and complex data structures with comprehensive error reporting.

## Features

- **Type-safe validation** with compile-time checking
- **Fluent API** for easy and readable validation rules
- **Comprehensive error reporting** with detailed error messages and paths
- **Extensible architecture** for custom validators
- **Support for primitive types** (string, numeric, boolean, DateTime, enum)
- **Complex type validation** for objects and collections
- **Conditional validation** with When/Unless patterns
- **Union types** for validating one of multiple possible types
- **Nullable and optional value support**
- **Custom validation functions**
- **Exception handling** with ValidationException

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
}
```

## Validator Types

### String Validation

```csharp
var stringValidator = Validator.String()
    .NotNull()
    .NotEmpty()
    .NotWhiteSpace()
    .MinLength(5)
    .MaxLength(100)
    .Length(10) // Exact length
    .Length(5, 15) // Length range
    .Matches(@"^[a-zA-Z]+$") // Regex pattern
    .Contains("test")
    .StartsWith("Hello")
    .EndsWith("World")
    .Email() // Email format
    .Url() // URL format
    .Guid() // GUID format
    .OneOf(new[] { "option1", "option2" }); // Allowed values
```

### Numeric Validation

```csharp
var intValidator = Validator.Int()
    .GreaterThan(0)
    .GreaterThanOrEqual(1)
    .LessThan(100)
    .LessThanOrEqual(99)
    .Between(1, 99)
    .Equal(42)
    .NotEqual(0)
    .Positive()
    .Negative()
    .Zero()
    .NotZero()
    .OneOf(new[] { 1, 2, 3, 5, 8 });

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
    .IsTrue()
    .IsFalse()
    .Equal(true);
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
    .NotNull()
    .NotEmpty()
    .MinCount(1)
    .MaxCount(10)
    .Count(5) // Exact count
    .Count(3, 7) // Count range
    .Unique() // All items must be unique
    .Contains("required-item")
    .DoesNotContain("forbidden-item")
    .ForEach(Validator.String().NotEmpty()) // Validate each item
    .All(item => item.StartsWith("prefix")) // All items must satisfy condition
    .Any(item => item.Contains("keyword")); // At least one item must satisfy condition

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
}

var userValidator = Validator.Object<User>()
    .NotNull()
    .RuleFor(u => u.Name, Validator.String().NotEmpty().MinLength(2))
    .RuleFor(u => u.Email, Validator.String().Email())
    .RuleFor(u => u.Age, Validator.Int().Between(0, 150))
    .RuleFor(u => u.DateOfBirth, Validator.DateTime().InThePast())
    .RuleFor(u => u.Tags, Validator.Array<string>().MaxCount(5)
        .ForEach(Validator.String().NotEmpty()))
    .Must(u => u.Age >= 18, "User must be at least 18 years old");
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
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"- {error.Message} (Code: {error.ErrorCode})");
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
if (validator.TryValidate("Hello", out var result))
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
    public static ValidationResult Combine(params ValidationResult[] results);
}

public class ValidationError
{
    public string Message { get; }
    public string? PropertyName { get; }
    public string? PropertyPath { get; }
    public string? ErrorCode { get; }
    public object? Context { get; }
    
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
- **`ValidationError`**: Immutable error with detailed information
- **`ValidationException`**: Exception thrown by `ValidateAndThrow`

### Validator Types

- **`StringValidator`**: String-specific validation rules
- **`NumericValidator<T>`**: Generic numeric validation for all numeric types
- **`BooleanValidator`**: Boolean validation rules
- **`DateTimeValidator`**: DateTime-specific validation
- **`EnumValidator<T>`**: Enum validation with type safety
- **`CollectionValidator<TCollection, TItem>`**: Generic collection validation
- **`ObjectValidator<T>`**: Complex object validation with property rules
- **`CustomValidator<T>`**: Custom validation functions
- **`NullableValidator<T>`**: Nullable value type validation
- **`OptionalValidator<T>`**: Nullable reference type validation
- **`UnionValidator`**: One-of-multiple-types validation

## Best Practices

1. **Reuse validators**: Create validator instances once and reuse them
2. **Use meaningful error messages**: Provide clear, actionable error messages
3. **Validate early**: Validate input as soon as possible
4. **Use property paths**: For complex objects, property paths help identify error locations
5. **Handle exceptions**: Use try-catch or TryValidate for graceful error handling
6. **Combine validators**: Use conditional and combined validators for complex scenarios

## Performance Considerations

- Validators are lightweight and can be cached/reused
- Property expressions are compiled once and cached
- Regular expressions are compiled for better performance
- Use `TryValidate` to avoid exceptions in hot paths

## Thread Safety

All validators are thread-safe and can be used concurrently. Validator instances are immutable after creation.

## License

This library is released under the MIT License. 