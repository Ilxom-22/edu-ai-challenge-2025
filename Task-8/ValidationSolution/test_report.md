# ValidationLibrary Test Report

This document provides a comprehensive overview of the test suite for the ValidationLibrary project, including test coverage, test descriptions, and comprehensive unit tests for all validators.

## How to Run Tests

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension

### Command Line Instructions

#### 1. Restore Dependencies
```bash
dotnet restore
```

#### 2. Build the Solution
```bash
dotnet build
```

#### 3. Run All Tests
```bash
dotnet test
```

#### 4. Run Specific Test Class
```bash
dotnet test --filter "StringValidatorTests"
```

#### 5. Run Specific Test Method
```bash
dotnet test --filter "NotEmpty_With_Valid_String_Should_Pass"
```

#### 6. Run Tests with Coverage (optional)
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory:TestResults
```

#### 7. Generate HTML Coverage Report (optional)
```bash
# Install ReportGenerator (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"TestResults\[guid]\coverage.cobertura.xml" -targetdir:"TestResults\coveragereport" -reporttypes:Html
```

### Visual Studio Instructions

1. **Open Solution**: Open `ValidationSolution.sln` in Visual Studio
2. **Build Solution**: Build > Build Solution (Ctrl+Shift+B)
3. **Run Tests**: Test > Run All Tests (Ctrl+R, A)
4. **View Test Explorer**: Test > Test Explorer
5. **Run Coverage**: Test > Analyze Code Coverage for All Tests

### VS Code Instructions

1. **Open Workspace**: Open the solution folder in VS Code
2. **Install Extensions**: Ensure C# and .NET Test Explorer extensions are installed
3. **Run Tests**: Use Test Explorer panel or run `dotnet test` in terminal
4. **Debug Tests**: Set breakpoints and use debug options in Test Explorer

## Overview


## Test Coverage Summary

### Overall Coverage
- **Total Tests**: 319 tests
- **Passing Tests**: 319 tests
- **Failing Tests**: 0 tests
- **Test Success Rate**: 100%

### Coverage by Component

| Component | Line Coverage | Branch Coverage | Description |
|-----------|---------------|-----------------|-------------|
| Core Classes | ~85% | ~70% | ValidationResult, ValidationError, BaseValidator |
| String Validator | ~90% | ~85% | String validation rules |
| Numeric Validators | ~88% | ~80% | Integer, Long, Double, Decimal, etc. |
| Boolean Validator | ~93% | ~100% | Boolean validation rules |
| Collection Validators | ~85% | ~75% | Array, List, IEnumerable validation |
| Object Validator | ~80% | ~70% | Complex object validation |
| DateTime Validator | ~95% | ~90% | Date and time validation with comprehensive tests |
| Enum Validator | ~95% | ~85% | Enum validation with comprehensive tests |
| Union Validator | ~90% | ~80% | Union type validation with comprehensive tests |
| Custom Validator | ~90% | ~80% | Custom validation with comprehensive tests |

## Test Structure

### Test Projects
- **ValidationLibrary.Tests**: Main test project with comprehensive unit tests

### Test Categories

#### 1. Core Component Tests (`ValidationLibrary.Tests.Core`)
- **ValidationResultTests**: Tests for ValidationResult class
  - Success and failure scenarios
  - Error combination
  - ToString formatting
  - Static factory methods
  
- **ValidationErrorTests**: Tests for ValidationError class
  - Constructor overloads with null/empty message handling
  - Property validation
  - Immutable operations (WithPath, WithCode, WithContext)
  - Equality comparison
  - String representation

#### 2. String Validator Tests (`ValidationLibrary.Tests.Validators.StringValidatorTests`)
- **Basic Validation**: NotNull, NotEmpty, NotWhiteSpace
- **Length Validation**: MinLength, MaxLength, exact Length, Length ranges
- **Pattern Matching**: Regex patterns, Contains, StartsWith, EndsWith
- **Format Validation**: Email, URL, GUID validation
- **Value Restrictions**: OneOf allowed values
- **Chaining**: Multiple validation rules combined
- **Error Handling**: ValidateAndThrow, TryValidate

#### 3. Numeric Validator Tests (`ValidationLibrary.Tests.Validators.NumericValidatorTests`)
- **Comparison Validation**: GreaterThan, LessThan, Between, Equal, NotEqual
- **Sign Validation**: Positive, Negative, Zero, NotZero
- **Value Restrictions**: OneOf allowed values
- **Type Coverage**: Int, Long, Double, Decimal, Float, Short, Byte
- **Chaining**: Multiple numeric rules combined

#### 4. Boolean Validator Tests (`ValidationLibrary.Tests.Validators.BooleanValidatorTests`)
- **Truth Validation**: IsTrue, IsFalse
- **Equality**: Equal validation
- **Custom Messages**: Error message customization
- **Chaining**: Multiple boolean rules

#### 5. Collection Validator Tests (`ValidationLibrary.Tests.Validators.CollectionValidatorTests`)
- **Null/Empty Validation**: NotNull, NotEmpty
- **Count Validation**: MinCount, MaxCount, exact Count, Count ranges
- **Content Validation**: Contains, DoesNotContain, Unique
- **Item Validation**: ForEach with nested validators
- **Predicate Validation**: All, Any with custom conditions
- **Type Coverage**: Array, List, IEnumerable, ICollection

#### 6. Object Validator Tests (`ValidationLibrary.Tests.Validators.ObjectValidatorTests`)
- **Basic Validation**: NotNull
- **Property Rules**: RuleFor with validators and predicates
- **Object-Level Rules**: Must validation
- **Conditional Validation**: When, Unless conditions
- **Complex Scenarios**: Nested validation, property paths
- **Error Aggregation**: Multiple property failures

#### 7. DateTime Validator Tests (`ValidationLibrary.Tests.Validators.DateTimeValidatorTests`)
- **Comparison Validation**: After, Before, OnOrAfter, OnOrBefore, Between
- **Time Context**: InThePast, InTheFuture, IsToday
- **Special Values**: NotDefault, WithinDaysFromToday
- **DateTime Properties**: HasKind validation
- **Chaining**: Multiple DateTime rules combined
- **Error Handling**: ValidateAndThrow, TryValidate

#### 8. Enum Validator Tests (`ValidationLibrary.Tests.Validators.EnumValidatorTests`)
- **Definition Validation**: IsDefined for valid enum values
- **Value Restrictions**: OneOf, NotOneOf, Equal, NotEqual
- **Flag Enums**: HasFlag, DoesNotHaveFlag for [Flags] enums
- **Theory Tests**: All valid enum and flag values
- **Chaining**: Multiple enum validation rules
- **Custom Messages**: Error message customization

#### 9. Custom Validator Tests (`ValidationLibrary.Tests.Validators.CustomValidatorTests`)
- **Predicate Validation**: Simple boolean predicates with custom messages
- **ValidationResult Functions**: Complex validation with detailed errors
- **Error Handling**: Exception handling in custom validation
- **Chaining**: And method for combining custom validations
- **Error Codes**: Custom error code support
- **Type Coverage**: Reference types, value types, nullable types

#### 10. Union Validator Tests (`ValidationLibrary.Tests.Validators.UnionValidatorTests`)
- **Type Alternatives**: String|Int, Object|Object unions
- **Complex Types**: Person|Address unions
- **Collection Types**: Array|List unions
- **Error Handling**: Type mismatch scenarios
- **Generic Unions**: Two-type specific unions
- **Extension**: Or method for adding additional validators
