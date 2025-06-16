# ValidationLibrary

A robust, type-safe validation library for .NET 8+ inspired by Zod (JavaScript), Pydantic (Python), and FluentValidation (.NET). This library provides a fluent API for validating primitive and complex data structures with comprehensive error reporting and excellent developer experience.

## 📋 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Core Concepts](#-core-concepts)
- [Validator Types](#-validator-types)
  - [String Validation](#string-validation)
  - [Numeric Validation](#numeric-validation)
  - [Boolean Validation](#boolean-validation)
  - [DateTime Validation](#datetime-validation)
  - [Enum Validation](#enum-validation)
  - [Collection Validation](#collection-validation)
  - [Object Validation](#object-validation)
  - [Custom Validation](#custom-validation)
  - [Nullable and Optional Values](#nullable-and-optional-values)
  - [Union Types](#union-types)
- [Error Handling](#-error-handling)
- [Advanced Features](#-advanced-features)
- [Real-World Examples](#-real-world-examples)
- [Performance & Best Practices](#-performance--best-practices)
- [Extensibility](#-extensibility)

## 🚀 Features

- **Type-safe validation** with compile-time checking
- **Fluent API** for easy and readable validation rules
- **Comprehensive error reporting** with detailed error messages and property paths
- **Extensible architecture** for custom validators
- **Support for all primitive types** (string, numeric, boolean, DateTime, enum)
- **Complex type validation** for objects and collections
- **Conditional validation** with When/Unless patterns
- **Union types** for validating one of multiple possible types
- **Nullable and optional value support**
- **Custom validation functions** with full flexibility
- **Exception handling** with ValidationException
- **Thread-safe** and **immutable** design
- **Zero dependencies** beyond .NET 8

## 📦 Installation

Add the ValidationLibrary to your project:

```bash
dotnet add package ValidationLibrary
```

Or via Package Manager Console:

```powershell
Install-Package ValidationLibrary
```

## ⚡ Quick Start

```csharp
using ValidationLibrary;

// Simple string validation
var emailValidator = Validator.String()
    .NotEmpty()
    .Email();

var result = emailValidator.Validate("user@example.com");
if (result.IsValid)
{
    Console.WriteLine("Valid email!");
}
else
{
    Console.WriteLine($"Invalid: {result.ErrorMessage}");
}

// Object validation
var userValidator = Validator.Object<User>()
    .NotNull()
    .RuleFor(u => u.Name, Validator.String().NotEmpty().MinLength(2))
    .RuleFor(u => u.Age, Validator.Int().Between(18, 120));

var user = new User { Name = "John", Age = 25 };
var userResult = userValidator.Validate(user);
```

## 🧠 Core Concepts

### ValidationResult

Every validation operation returns a `ValidationResult` containing:

- `IsValid` - Boolean indicating success/failure
- `Errors` - Collection of detailed validation errors
- `ErrorMessage` - First error message (null if valid)

### ValidationError

Each error contains:

- `Message` - Human-readable error description
- `PropertyName` - Name of the property that failed
- `PropertyPath` - Full path for nested properties
- `ErrorCode` - Optional code for programmatic handling
- `Context` - Additional metadata

### Fluent Validation Chain

Validators can be chained together:

```csharp
var validator = Validator.String()
    .NotEmpty()                    // First rule
    .MinLength(3)                  // Second rule
    .MaxLength(50)                 // Third rule
    .Matches(@"^[a-zA-Z\s]+$");    // Fourth rule
```

## 📚 Validator Types

### String Validation

Comprehensive string validation with common patterns:

```csharp
var stringValidator = Validator.String()
    .NotNull()                              // Cannot be null
    .NotEmpty()                             // Cannot be null or empty
    .NotWhiteSpace()                        // Cannot be null, empty, or whitespace
    .MinLength(5)                           // Minimum length
    .MaxLength(100)                         // Maximum length
    .Length(10)                             // Exact length
    .Length(5, 15)                          // Length range
    .Matches(@"^[a-zA-Z]+$")                // Regex pattern
    .Contains("substring")                   // Contains text
    .StartsWith("prefix")                   // Starts with text
    .EndsWith("suffix")                     // Ends with text
    .Email()                                // Valid email format
    .Url()                                  // Valid URL format
    .Guid()                                 // Valid GUID format
    .OneOf(new[] { "option1", "option2" }); // Allowed values

// Real-world examples
var nameValidator = Validator.String()
    .NotEmpty()
    .MinLength(2)
    .MaxLength(100)
    .Matches(@"^[a-zA-Z\s'-]+$", "Name can only contain letters, spaces, hyphens, and apostrophes");

var passwordValidator = Validator.String()
    .NotEmpty()
    .MinLength(8)
    .Matches(@"(?=.*[a-z])", "Must contain lowercase letter")
    .Matches(@"(?=.*[A-Z])", "Must contain uppercase letter")
    .Matches(@"(?=.*\d)", "Must contain digit")
    .Matches(@"(?=.*[@$!%*?&])", "Must contain special character");

var phoneValidator = Validator.String()
    .NotEmpty()
    .Matches(@"^\+?[\d\s\-\(\)]+$", "Must be a valid phone number");
```

### Numeric Validation

Type-safe numeric validation for all numeric types:

```csharp
// Integer validation
var ageValidator = Validator.Int()
    .GreaterThan(0)                         // Exclusive minimum
    .GreaterThanOrEqual(18)                 // Inclusive minimum
    .LessThan(150)                          // Exclusive maximum
    .LessThanOrEqual(120)                   // Inclusive maximum
    .Between(18, 120)                       // Range (inclusive)
    .Equal(42)                              // Exact value
    .NotEqual(0)                            // Not equal to value
    .Positive()                             // Greater than zero
    .Negative()                             // Less than zero
    .Zero()                                 // Equal to zero
    .NotZero()                              // Not equal to zero
    .OneOf(new[] { 1, 2, 3, 5, 8 });       // Allowed values

// Other numeric types
var priceValidator = Validator.Decimal()
    .GreaterThan(0)
    .LessThanOrEqual(1000000);

var percentageValidator = Validator.Double()
    .Between(0.0, 100.0);

var scoreValidator = Validator.Float()
    .Between(0.0f, 10.0f);

// Real-world examples
var salaryValidator = Validator.Decimal()
    .GreaterThanOrEqual(0)
    .LessThanOrEqual(10000000);

var ratingValidator = Validator.Int()
    .Between(1, 5);

var temperatureValidator = Validator.Double()
    .Between(-273.15, 1000);
```

### Boolean Validation

Simple boolean validation:

```csharp
var boolValidator = Validator.Boolean()
    .IsTrue()                               // Must be true
    .IsFalse()                              // Must be false
    .Equal(true);                           // Must equal specific value

// Real-world examples
var termsAcceptedValidator = Validator.Boolean()
    .IsTrue("Terms and conditions must be accepted");

var isActiveValidator = Validator.Boolean()
    .Equal(true, "User must be active");
```

### DateTime Validation

Comprehensive date and time validation:

```csharp
var dateValidator = Validator.DateTime()
    .After(DateTime.Now)                    // Future date
    .Before(DateTime.Now.AddYears(1))       // Before specific date
    .OnOrAfter(DateTime.Today)              // Today or later
    .OnOrBefore(DateTime.Today.AddDays(30)) // Within 30 days
    .Between(DateTime.Now, DateTime.Now.AddYears(1)) // Date range
    .InThePast()                            // Must be in the past
    .InTheFuture()                          // Must be in the future
    .IsToday()                              // Must be today
    .NotDefault()                           // Not DateTime.MinValue
    .WithinDaysFromToday(30)                // Within X days from today
    .HasKind(DateTimeKind.Utc);             // Specific DateTimeKind

// Real-world examples
var birthDateValidator = Validator.DateTime()
    .Before(DateTime.Today.AddYears(-13))   // At least 13 years old
    .After(DateTime.Today.AddYears(-150));  // Not older than 150 years

var appointmentValidator = Validator.DateTime()
    .After(DateTime.Now.AddHours(1))        // At least 1 hour in future
    .Before(DateTime.Now.AddDays(365));     // Within next year

var eventDateValidator = Validator.DateTime()
    .OnOrAfter(DateTime.Today)              // Today or future
    .HasKind(DateTimeKind.Utc);            // UTC timezone
```

### Enum Validation

Type-safe enum validation:

```csharp
public enum Status { Active, Inactive, Pending, Suspended }
public enum Priority { Low = 1, Medium = 2, High = 3, Critical = 4 }

var statusValidator = Validator.Enum<Status>()
    .IsDefined()                            // Valid enum value
    .OneOf(new[] { Status.Active, Status.Pending }) // Allowed values
    .NotEqual(Status.Suspended)             // Forbidden value
    .Equal(Status.Active);                  // Specific value

// Flags enum support
[Flags]
public enum Permissions { Read = 1, Write = 2, Execute = 4, Delete = 8 }

var permissionsValidator = Validator.Enum<Permissions>()
    .HasFlag(Permissions.Read)              // Must have specific flag
    .DoesNotHaveFlag(Permissions.Delete);   // Must not have flag

// Real-world examples
var orderStatusValidator = Validator.Enum<OrderStatus>()
    .IsDefined()
    .NotOneOf(new[] { OrderStatus.Cancelled, OrderStatus.Refunded });

var userRoleValidator = Validator.Enum<UserRole>()
    .OneOf(new[] { UserRole.User, UserRole.Moderator, UserRole.Admin });
```

### Collection Validation

Powerful collection validation with per-item validation:

```csharp
var arrayValidator = Validator.Array<string>()
    .NotNull()                              // Array not null
    .NotEmpty()                             // At least one item
    .MinCount(1)                            // Minimum items
    .MaxCount(10)                           // Maximum items
    .Count(5)                               // Exact count
    .Count(3, 7)                            // Count range
    .Unique()                               // All items unique
    .Contains("required-item")              // Must contain item
    .DoesNotContain("forbidden-item")       // Must not contain item
    .ForEach(Validator.String().NotEmpty()) // Validate each item
    .All(item => item.StartsWith("prefix")) // All items satisfy condition
    .Any(item => item.Contains("keyword")); // At least one item satisfies

// Other collection types
var listValidator = Validator.List<int>()
    .NotNull()
    .MaxCount(100)
    .ForEach(Validator.Int().Positive());

var enumerableValidator = Validator.Enumerable<double>()
    .NotNull()
    .All(value => value >= 0);

// Real-world examples
var tagsValidator = Validator.Array<string>()
    .NotNull()
    .MaxCount(10)
    .ForEach(Validator.String()
        .NotEmpty()
        .MaxLength(50)
        .Matches(@"^[a-zA-Z0-9\-_]+$", "Tags can only contain alphanumeric characters, hyphens, and underscores"));

var emailListValidator = Validator.List<string>()
    .NotNull()
    .MinCount(1)
    .MaxCount(50)
    .Unique("Duplicate emails are not allowed")
    .ForEach(Validator.String().Email());

var scoresValidator = Validator.Array<int>()
    .NotNull()
    .Count(5, 20)
    .ForEach(Validator.Int().Between(0, 100))
    .All(score => score >= 60, "All scores must be passing (60 or above)");
```

### Object Validation

Complex object validation with property-level rules:

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string[] Tags { get; set; }
}

var userValidator = Validator.Object<User>()
    .NotNull()
    .RuleFor(u => u.Name, Validator.String()
        .NotEmpty()
        .MinLength(2)
        .MaxLength(100))
    .RuleFor(u => u.Email, Validator.String()
        .NotEmpty()
        .Email())
    .RuleFor(u => u.Age, Validator.Int()
        .Between(13, 120))
    .RuleFor(u => u.DateOfBirth, Validator.DateTime()
        .Before(DateTime.Today.AddYears(-13)))
    .RuleFor(u => u.Address, addressValidator) // Nested object
    .RuleFor(u => u.Tags, Validator.Array<string>()
        .MaxCount(10)
        .ForEach(Validator.String().NotEmpty()))
    .Must(u => u.Age >= 18 || u.HasParentalConsent, 
          "Users under 18 must have parental consent");

// Conditional validation
var conditionalValidator = Validator.Object<User>()
    .RuleFor(u => u.Name, Validator.String().NotEmpty())
    .When(u => u.Age >= 18, validator =>
        validator.RuleFor(u => u.Email, Validator.String().Email())
                 .RuleFor(u => u.PhoneNumber, Validator.String().NotEmpty()))
    .Unless(u => u.IsGuest, validator =>
        validator.RuleFor(u => u.Password, passwordValidator));

// Cross-property validation
var orderValidator = Validator.Object<Order>()
    .RuleFor(o => o.StartDate, Validator.DateTime().InTheFuture())
    .RuleFor(o => o.EndDate, Validator.DateTime().InTheFuture())
    .Must(o => o.EndDate > o.StartDate, "End date must be after start date")
    .Must(o => o.TotalAmount == o.Items.Sum(i => i.Price), 
          "Total amount must equal sum of item prices");
```

### Custom Validation

Flexible custom validation for complex business rules:

```csharp
// Custom validator with function
var customValidator = Validator.Custom<string>(value =>
{
    if (string.IsNullOrEmpty(value))
        return ValidationResult.Failure("Value cannot be empty");
    
    if (!IsValidBusinessRule(value))
        return ValidationResult.Failure("Business rule validation failed");
    
    return ValidationResult.Success();
});

// Custom validator with predicate
var evenLengthValidator = Validator.Custom<string>(
    value => value?.Length % 2 == 0,
    "String length must be even");

// Complex custom validation
var accountNumberValidator = Validator.Custom<string>(accountNumber =>
{
    if (string.IsNullOrWhiteSpace(accountNumber))
        return ValidationResult.Failure("Account number is required");
    
    if (accountNumber.Length != 10)
        return ValidationResult.Failure("Account number must be 10 digits");
    
    if (!accountNumber.All(char.IsDigit))
        return ValidationResult.Failure("Account number must contain only digits");
    
    // Luhn algorithm check
    if (!IsValidLuhnChecksum(accountNumber))
        return ValidationResult.Failure("Invalid account number checksum");
    
    return ValidationResult.Success();
});

// Async custom validation (for external API calls)
var uniqueEmailValidator = Validator.Custom<string>(async email =>
{
    if (await userService.EmailExistsAsync(email))
        return ValidationResult.Failure("Email already exists");
    
    return ValidationResult.Success();
});
```

### Nullable and Optional Values

Handle nullable values elegantly:

```csharp
// Nullable value types
var nullableAgeValidator = Validator.Nullable(Validator.Int().Positive())
    .Required("Age is required"); // Make it required

// Optional reference types
var optionalNameValidator = Validator.Optional(Validator.String().NotEmpty())
    .Required("Name is required when provided");

// Real-world example
var optionalPhoneValidator = Validator.Optional(Validator.String()
    .Matches(@"^\+?[\d\s\-\(\)]+$", "Invalid phone format"));

var nullableBirthDateValidator = Validator.Nullable(Validator.DateTime()
    .Before(DateTime.Today));
```

### Union Types

Validate one of multiple possible types:

```csharp
// Union of two types
var stringOrIntValidator = new UnionValidator<string, int>(
    Validator.String().NotEmpty(),
    Validator.Int().Positive()
);

// General union validator
var multiTypeValidator = Validator.Union(
    Validator.String().Email(),
    Validator.Int().Between(1, 1000),
    Validator.Boolean()
);

// Real-world example: API parameter that can be ID or name
var resourceIdentifierValidator = new UnionValidator<string, int>(
    Validator.String().NotEmpty().MinLength(3), // Resource name
    Validator.Int().Positive()                   // Resource ID
);
```

## 🚨 Error Handling

### Basic Error Handling

```csharp
var validator = Validator.String().NotEmpty().MinLength(5);

// Check validation result
var result = validator.Validate("Hi");
if (!result.IsValid)
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
    
    // Access all errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Property: {error.PropertyPath}");
        Console.WriteLine($"Message: {error.Message}");
        Console.WriteLine($"Code: {error.ErrorCode}");
    }
}
```

### Exception-Based Validation

```csharp
try
{
    var validatedValue = validator.ValidateAndThrow("Hi");
    // Use validatedValue here
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
    
    // Access validation result
    var result = ex.ValidationResult;
    foreach (var error in ex.Errors)
    {
        LogError(error.PropertyPath, error.Message);
    }
}
```

### Try-Validate Pattern

```csharp
if (validator.TryValidate("Hello", out var result))
{
    Console.WriteLine("Validation succeeded");
    // Use the value
}
else
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
}
```

### Combining Multiple Validations

```csharp
var nameResult = nameValidator.Validate(name);
var emailResult = emailValidator.Validate(email);
var ageResult = ageValidator.Validate(age);

var combinedResult = Validator.Combine(nameResult, emailResult, ageResult);

if (!combinedResult.IsValid)
{
    // Handle all validation errors together
    foreach (var error in combinedResult.Errors)
    {
        Console.WriteLine($"Field {error.PropertyPath}: {error.Message}");
    }
}
```

## 🔥 Advanced Features

### Conditional Validation

```csharp
var userValidator = Validator.Object<User>()
    .RuleFor(u => u.Name, Validator.String().NotEmpty())
    .When(u => u.IsVIP, validator =>
        validator.RuleFor(u => u.VIPCode, Validator.String().NotEmpty()))
    .Unless(u => u.IsExternalUser, validator =>
        validator.RuleFor(u => u.InternalId, Validator.String().NotEmpty()));
```

### Cross-Property Validation

```csharp
var changePasswordValidator = Validator.Object<ChangePasswordRequest>()
    .RuleFor(r => r.NewPassword, passwordValidator)
    .RuleFor(r => r.ConfirmPassword, Validator.String().NotEmpty())
    .Must(r => r.NewPassword == r.ConfirmPassword, 
          "Password confirmation must match new password")
    .Must(r => r.NewPassword != r.CurrentPassword, 
          "New password must be different from current password");
```

### Transformation Validation

```csharp
// Transform and validate
var intFromStringValidator = Validator.String()
    .NotEmpty()
    .Transform<int>(int.Parse)
    .Positive();

// Custom transformation
var trimmedStringValidator = Validator.String()
    .Transform<string>(s => s?.Trim())
    .NotEmpty()
    .MinLength(3);
```

### Custom Error Messages

```csharp
var validator = Validator.String()
    .NotEmpty()
    .WithMessage("Please provide a value");

var contextualMessageValidator = Validator.Int()
    .GreaterThan(0)
    .WithMessage((value, result) => $"Expected positive number, got {value}");
```

## 🌍 Real-World Examples

### User Registration Validation

```csharp
public class UserRegistrationRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public bool AcceptTerms { get; set; }
    public string[] Interests { get; set; }
}

var registrationValidator = Validator.Object<UserRegistrationRequest>()
    .NotNull()
    .RuleFor(r => r.Username, Validator.String()
        .NotEmpty("Username is required")
        .MinLength(3, "Username must be at least 3 characters")
        .MaxLength(30, "Username cannot exceed 30 characters")
        .Matches(@"^[a-zA-Z0-9_]+$", "Username can only contain letters, numbers, and underscores"))
    .RuleFor(r => r.Email, Validator.String()
        .NotEmpty("Email is required")
        .Email("Please provide a valid email address"))
    .RuleFor(r => r.Password, Validator.String()
        .NotEmpty("Password is required")
        .MinLength(8, "Password must be at least 8 characters")
        .Matches(@"(?=.*[a-z])", "Password must contain at least one lowercase letter")
        .Matches(@"(?=.*[A-Z])", "Password must contain at least one uppercase letter")
        .Matches(@"(?=.*\d)", "Password must contain at least one digit")
        .Matches(@"(?=.*[@$!%*?&])", "Password must contain at least one special character"))
    .RuleFor(r => r.ConfirmPassword, Validator.String()
        .NotEmpty("Password confirmation is required"))
    .RuleFor(r => r.DateOfBirth, Validator.DateTime()
        .Before(DateTime.Today.AddYears(-13), "Must be at least 13 years old")
        .After(DateTime.Today.AddYears(-120), "Invalid birth date"))
    .RuleFor(r => r.PhoneNumber, Validator.String()
        .NotEmpty("Phone number is required")
        .Matches(@"^\+?[\d\s\-\(\)]+$", "Please provide a valid phone number"))
    .RuleFor(r => r.AcceptTerms, Validator.Boolean()
        .IsTrue("You must accept the terms and conditions"))
    .RuleFor(r => r.Interests, Validator.Array<string>()
        .NotNull("Interests cannot be null")
        .MinCount(1, "Please select at least one interest")
        .MaxCount(10, "You can select up to 10 interests")
        .ForEach(Validator.String()
            .NotEmpty("Interest cannot be empty")
            .MaxLength(50, "Interest name too long")))
    .Must(r => r.Password == r.ConfirmPassword, 
          "Password confirmation must match password");
```

### E-commerce Order Validation

```csharp
public class Order
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public string CouponCode { get; set; }
    public DateTime OrderDate { get; set; }
}

var orderValidator = Validator.Object<Order>()
    .NotNull()
    .RuleFor(o => o.OrderId, Validator.String()
        .NotEmpty("Order ID is required")
        .Matches(@"^ORD-\d{8}$", "Order ID must be in format ORD-12345678"))
    .RuleFor(o => o.CustomerId, Validator.String()
        .NotEmpty("Customer ID is required")
        .Guid("Customer ID must be a valid GUID"))
    .RuleFor(o => o.Items, Validator.List<OrderItem>()
        .NotNull("Order items cannot be null")
        .NotEmpty("Order must contain at least one item")
        .MaxCount(50, "Order cannot contain more than 50 items")
        .ForEach(orderItemValidator))
    .RuleFor(o => o.ShippingAddress, addressValidator)
    .RuleFor(o => o.BillingAddress, addressValidator)
    .RuleFor(o => o.PaymentMethod, Validator.Enum<PaymentMethod>()
        .IsDefined("Invalid payment method"))
    .RuleFor(o => o.TotalAmount, Validator.Decimal()
        .GreaterThan(0, "Total amount must be greater than zero")
        .LessThanOrEqual(100000, "Total amount cannot exceed $100,000"))
    .RuleFor(o => o.CouponCode, Validator.Optional(Validator.String()
        .Matches(@"^[A-Z0-9]{6,12}$", "Invalid coupon code format")))
    .RuleFor(o => o.OrderDate, Validator.DateTime()
        .OnOrBefore(DateTime.Now, "Order date cannot be in the future"))
    .Must(o => o.TotalAmount == o.Items.Sum(i => i.Price * i.Quantity), 
          "Total amount must equal sum of item prices")
    .Must(o => o.Items.All(i => i.Quantity > 0), 
          "All items must have positive quantity");
```

### API Request Validation

```csharp
public class SearchRequest
{
    public string Query { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string[] Filters { get; set; }
    public string SortBy { get; set; }
    public string SortOrder { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

var searchRequestValidator = Validator.Object<SearchRequest>()
    .NotNull()
    .RuleFor(r => r.Query, Validator.String()
        .NotEmpty("Search query is required")
        .MinLength(2, "Query must be at least 2 characters")
        .MaxLength(100, "Query cannot exceed 100 characters"))
    .RuleFor(r => r.Page, Validator.Int()
        .GreaterThanOrEqual(1, "Page must be 1 or greater"))
    .RuleFor(r => r.PageSize, Validator.Int()
        .Between(1, 100, "Page size must be between 1 and 100"))
    .RuleFor(r => r.Filters, Validator.Array<string>()
        .MaxCount(20, "Too many filters")
        .ForEach(Validator.String()
            .NotEmpty()
            .Matches(@"^[a-zA-Z]+:[a-zA-Z0-9\-_]+$", "Invalid filter format")))
    .RuleFor(r => r.SortBy, Validator.String()
        .OneOf(new[] { "name", "date", "price", "rating" }, "Invalid sort field"))
    .RuleFor(r => r.SortOrder, Validator.String()
        .OneOf(new[] { "asc", "desc" }, "Sort order must be 'asc' or 'desc'"))
    .RuleFor(r => r.DateFrom, Validator.Nullable(Validator.DateTime()
        .OnOrBefore(DateTime.Today, "Date from cannot be in the future")))
    .RuleFor(r => r.DateTo, Validator.Nullable(Validator.DateTime()
        .OnOrBefore(DateTime.Today, "Date to cannot be in the future")))
    .When(r => r.DateFrom.HasValue && r.DateTo.HasValue, validator =>
        validator.Must(r => r.DateTo >= r.DateFrom, 
                      "Date to must be on or after date from"));
```

### File Upload Validation

```csharp
public class FileUploadRequest
{
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public byte[] FileContent { get; set; }
}

var allowedContentTypes = new[]
{
    "image/jpeg", "image/png", "image/gif",
    "application/pdf", "text/plain",
    "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
};

var fileUploadValidator = Validator.Object<FileUploadRequest>()
    .NotNull()
    .RuleFor(f => f.FileName, Validator.String()
        .NotEmpty("File name is required")
        .MaxLength(255, "File name too long")
        .Matches(@"^[^<>:"/\\|?*]+$", "File name contains invalid characters"))
    .RuleFor(f => f.FileSize, Validator.Long()
        .GreaterThan(0, "File cannot be empty")
        .LessThanOrEqual(10 * 1024 * 1024, "File size cannot exceed 10MB"))
    .RuleFor(f => f.ContentType, Validator.String()
        .NotEmpty("Content type is required")
        .OneOf(allowedContentTypes, "File type not allowed"))
    .RuleFor(f => f.FileContent, Validator.Array<byte>()
        .NotNull("File content is required")
        .NotEmpty("File content cannot be empty"))
    .Must(f => f.FileContent.Length == f.FileSize, 
          "File size must match content length");
```

## ⚡ Performance & Best Practices

### Performance Tips

1. **Reuse Validators**: Create validator instances once and reuse them
```csharp
// Good: Create once, reuse many times
private static readonly IValidator<User> UserValidator = CreateUserValidator();

// Avoid: Creating validators repeatedly
var validator = Validator.String().NotEmpty(); // Don't do this in loops
```

2. **Use TryValidate for Hot Paths**: Avoid exceptions in performance-critical code
```csharp
// Hot path - no exceptions
if (validator.TryValidate(value, out var result))
{
    // Process valid value
}

// Cold path - exceptions are OK
try
{
    var validValue = validator.ValidateAndThrow(value);
}
catch (ValidationException ex)
{
    // Handle validation errors
}
```

3. **Compile Regex Patterns**: For frequently used patterns
```csharp
var emailValidator = Validator.String()
    .Matches(new Regex(@"^[^@]+@[^@]+\.[^@]+$", RegexOptions.Compiled));
```

### Best Practices

1. **Use Meaningful Error Messages**
```csharp
// Good
.MinLength(8, "Password must be at least 8 characters long")

// Bad
.MinLength(8)
```

2. **Validate Early and Often**
```csharp
// Validate at API boundaries
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var validationResult = userValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        // Process valid request
    }
}
```

3. **Use Property Paths for Complex Objects**
```csharp
// Access nested property errors
foreach (var error in result.Errors)
{
    if (error.PropertyPath == "Address.Country")
    {
        // Handle country validation error
    }
}
```

4. **Combine Validation Results**
```csharp
var results = new[]
{
    nameValidator.Validate(user.Name),
    emailValidator.Validate(user.Email),
    ageValidator.Validate(user.Age)
};

var combinedResult = Validator.Combine(results);
```

### Thread Safety

All validators are **thread-safe** and **immutable**. You can safely:
- Share validator instances across threads
- Use validators in concurrent scenarios
- Cache validators as static members

```csharp
// Thread-safe usage
private static readonly IValidator<User> SharedValidator = Validator.Object<User>()
    .RuleFor(u => u.Name, Validator.String().NotEmpty());

// Safe to use from multiple threads
Parallel.ForEach(users, user =>
{
    var result = SharedValidator.Validate(user);
    // Process result
});
```

## 🔧 Extensibility

### Custom Validators

Create reusable custom validators:

```csharp
public class CreditCardValidator : BaseValidator<string>
{
    protected override BaseValidator<string> CreateInstance() => new CreditCardValidator();
    
    public CreditCardValidator IsVisa(string message = "Must be a valid Visa card")
    {
        return (CreditCardValidator)AddRule(
            value => value?.StartsWith("4") == true && value.Length == 16,
            message,
            "CREDIT_CARD_VISA");
    }
    
    public CreditCardValidator IsMasterCard(string message = "Must be a valid MasterCard")
    {
        return (CreditCardValidator)AddRule(
            value => value?.StartsWith("5") == true && value.Length == 16,
            message,
            "CREDIT_CARD_MASTERCARD");
    }
    
    public CreditCardValidator PassesLuhnCheck(string message = "Invalid credit card number")
    {
        return (CreditCardValidator)AddRule(
            value => IsValidLuhnChecksum(value),
            message,
            "CREDIT_CARD_LUHN");
    }
    
    private static bool IsValidLuhnChecksum(string cardNumber)
    {
        // Luhn algorithm implementation
        // ... implementation details
        return true; // Simplified
    }
}

// Usage
var creditCardValidator = new CreditCardValidator()
    .NotEmpty()
    .IsVisa()
    .PassesLuhnCheck();
```

### Extension Methods

Extend existing validators:

```csharp
public static class StringValidatorExtensions
{
    public static StringValidator IsPhoneNumber(this StringValidator validator, 
        string message = "Must be a valid phone number")
    {
        return validator.Matches(@"^\+?[\d\s\-\(\)]+$", message);
    }
    
    public static StringValidator IsPostalCode(this StringValidator validator, 
        string country = "US", string message = "Invalid postal code")
    {
        var pattern = country.ToUpper() switch
        {
            "US" => @"^\d{5}(-\d{4})?$",
            "CA" => @"^[A-Z]\d[A-Z] \d[A-Z]\d$",
            "UK" => @"^[A-Z]{1,2}\d[A-Z\d]? \d[A-Z]{2}$",
            _ => throw new ArgumentException($"Unsupported country: {country}")
        };
        
        return validator.Matches(pattern, message);
    }
}

// Usage
var addressValidator = Validator.String()
    .NotEmpty()
    .IsPostalCode("US");

var phoneValidator = Validator.String()
    .NotEmpty()
    .IsPhoneNumber();
```

### Transformation Extensions

Create transformation validators:

```csharp
public static class TransformationExtensions
{
    public static IValidator<TInput> Transform<TInput, TOutput>(
        this IValidator<TOutput> validator,
        Func<TInput, TOutput> transform,
        string errorMessage = "Transformation failed")
    {
        return Validator.Custom<TInput>(input =>
        {
            try
            {
                var transformed = transform(input);
                return validator.Validate(transformed);
            }
            catch (Exception ex)
            {
                return ValidationResult.Failure($"{errorMessage}: {ex.Message}");
            }
        });
    }
}

// Usage
var stringToIntValidator = Validator.String()
    .NotEmpty()
    .Transform<string, int>(int.Parse, "Must be a valid integer")
    .GreaterThan(0);
```

### Custom Error Providers

Implement custom error message providers:

```csharp
public interface IErrorMessageProvider
{
    string GetMessage(string errorCode, object context);
}

public class LocalizedErrorProvider : IErrorMessageProvider
{
    private readonly IStringLocalizer _localizer;
    
    public LocalizedErrorProvider(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }
    
    public string GetMessage(string errorCode, object context)
    {
        return _localizer[errorCode, context].Value;
    }
}

// Custom validator with localization
public class LocalizedStringValidator : StringValidator
{
    private readonly IErrorMessageProvider _errorProvider;
    
    public LocalizedStringValidator(IErrorMessageProvider errorProvider)
    {
        _errorProvider = errorProvider;
    }
    
    public new LocalizedStringValidator NotEmpty()
    {
        var message = _errorProvider.GetMessage("STRING_NOT_EMPTY", null);
        return (LocalizedStringValidator)AddRule(
            value => !string.IsNullOrEmpty(value),
            message,
            "STRING_NOT_EMPTY");
    }
}
```

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📞 Support

If you encounter any issues or have questions:
- Create an issue on GitHub
- Check the documentation
- Review the examples in this README

---

**ValidationLibrary** - Making validation simple, type-safe, and powerful. ✨ 