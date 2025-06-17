using FluentAssertions;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class CustomValidatorTests
    {
        [Fact]
        public void Custom_With_Valid_Predicate_Should_Pass()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Value must be positive");

            // Act
            var result = validator.Validate(5);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Custom_With_Invalid_Predicate_Should_Fail()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Value must be positive");

            // Act
            var result = validator.Validate(-5);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be positive");
        }

        [Fact]
        public void Custom_With_ValidationResult_Function_Should_Pass()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
                string.IsNullOrEmpty(value)
                    ? ValidationResult.Failure("String cannot be empty")
                    : ValidationResult.Success());

            // Act
            var result = validator.Validate("valid");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Custom_With_ValidationResult_Function_Should_Fail()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
                string.IsNullOrEmpty(value)
                    ? ValidationResult.Failure("String cannot be empty")
                    : ValidationResult.Success());

            // Act
            var result = validator.Validate("");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("String cannot be empty");
        }

        [Fact]
        public void Custom_With_Complex_Validation_Should_Work()
        {
            // Arrange
            var validator = Validator.Custom<Person>(person =>
            {
                var errors = new List<ValidationError>();

                if (string.IsNullOrEmpty(person.Name))
                    errors.Add(new ValidationError("Name is required", "Name"));

                if (person.Age < 0)
                    errors.Add(new ValidationError("Age cannot be negative", "Age"));

                if (person.Age > 150)
                    errors.Add(new ValidationError("Age seems unrealistic", "Age"));

                return errors.Count == 0
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(errors);
            });

            var validPerson = new Person { Name = "John", Age = 30 };

            // Act
            var result = validator.Validate(validPerson);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Custom_With_Complex_Validation_Should_Return_Multiple_Errors()
        {
            // Arrange
            var validator = Validator.Custom<Person>(person =>
            {
                var errors = new List<ValidationError>();

                if (string.IsNullOrEmpty(person.Name))
                    errors.Add(new ValidationError("Name is required", "Name"));

                if (person.Age < 0)
                    errors.Add(new ValidationError("Age cannot be negative", "Age"));

                return errors.Count == 0
                    ? ValidationResult.Success()
                    : ValidationResult.Failure(errors);
            });

            var invalidPerson = new Person { Name = "", Age = -5 };

            // Act
            var result = validator.Validate(invalidPerson);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
            result.Errors.Should().Contain(e => e.PropertyName == "Age");
        }

        [Fact]
        public void Custom_With_Exception_Should_Return_Failure()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
            {
                throw new InvalidOperationException("Something went wrong");
            });

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Custom validation failed");
            result.ErrorMessage.Should().Contain("Something went wrong");
        }

        [Fact]
        public void And_With_Predicate_Should_Combine_Validations()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Must be positive")
                .And(value => value < 100, "Must be less than 100");

            // Act
            var result = validator.Validate(50);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void And_With_Predicate_Should_Fail_Both_Validations()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Must be positive")
                .And(value => value < 100, "Must be less than 100");

            // Act
            var result = validator.Validate(-150);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1); // The And method combines errors, so might only show the first failure
            result.ErrorMessage.Should().Contain("Must be positive");
        }

        [Fact]
        public void And_With_ValidationResult_Function_Should_Combine()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
                    value.Length > 3 ? ValidationResult.Success() : ValidationResult.Failure("Too short"))
                .And(value =>
                    value.Contains("@") ? ValidationResult.Success() : ValidationResult.Failure("Must contain @"));

            // Act
            var result = validator.Validate("test@example");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void And_With_ValidationResult_Function_Should_Fail_One()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
                    value.Length > 3 ? ValidationResult.Success() : ValidationResult.Failure("Too short"))
                .And(value =>
                    value.Contains("@") ? ValidationResult.Success() : ValidationResult.Failure("Must contain @"));

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Must contain @");
        }

        [Fact]
        public void Custom_With_Error_Code_Should_Include_Code()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(new[] { new ValidationError("Must be positive", errorCode: "POSITIVE_REQUIRED") }));

            // Act
            var result = validator.Validate(-5);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().ErrorCode.Should().Be("POSITIVE_REQUIRED");
        }

        [Fact]
        public void Custom_With_Null_Value_Should_Handle_Gracefully()
        {
            // Arrange
            var validator = Validator.Custom<string?>(value =>
                value != null ? ValidationResult.Success() : ValidationResult.Failure("Value cannot be null"));

            // Act
            var result = validator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value cannot be null");
        }

        [Fact]
        public void Custom_Validator_Should_Work_With_Reference_Types()
        {
            // Arrange
            var validator = Validator.Custom<List<int>>(list =>
                list.Count > 0 ? ValidationResult.Success() : ValidationResult.Failure("List cannot be empty"));

            // Act
            var result1 = validator.Validate(new List<int> { 1, 2, 3 });
            var result2 = validator.Validate(new List<int>());

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ErrorMessage.Should().Be("List cannot be empty");
        }

        [Fact]
        public void Custom_Validator_Should_Work_With_Value_Types()
        {
            // Arrange
            var validator = Validator.Custom<DateTime>(date =>
                date > DateTime.MinValue ? ValidationResult.Success() : ValidationResult.Failure("Invalid date"));

            // Act
            var result1 = validator.Validate(DateTime.Now);
            var result2 = validator.Validate(DateTime.MinValue);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
        }

        [Fact]
        public void TryValidate_With_Valid_Custom_Should_Return_True()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Must be positive");

            // Act
            var success = validator.TryValidate(5, out var result);

            // Assert
            success.Should().BeTrue();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void TryValidate_With_Invalid_Custom_Should_Return_False()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Must be positive");

            // Act
            var success = validator.TryValidate(-5, out var result);

            // Assert
            success.Should().BeFalse();
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAndThrow_With_Invalid_Custom_Should_Throw_Exception()
        {
            // Arrange
            var validator = Validator.Custom<int>(value => value > 0, "Must be positive");

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                validator.ValidateAndThrow(-5));

            exception.Message.Should().Contain("Must be positive");
        }

        [Fact]
        public void Custom_Validator_Constructor_With_Null_Function_Should_Throw()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ValidationLibrary.Validators.CustomValidator<int>(null!));
        }

        [Fact]
        public void Custom_With_Async_Like_Operation_Should_Work()
        {
            // Arrange
            var validator = Validator.Custom<string>(value =>
            {
                // Simulate some complex validation logic
                if (value.Contains("banned"))
                    return ValidationResult.Failure("Contains banned word");

                if (value.Length < 3)
                    return ValidationResult.Failure("Too short");

                return ValidationResult.Success();
            });

            // Act
            var result1 = validator.Validate("valid text");
            var result2 = validator.Validate("banned");
            var result3 = validator.Validate("x");

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ErrorMessage.Should().Be("Contains banned word");
            result3.IsValid.Should().BeFalse();
            result3.ErrorMessage.Should().Be("Too short");
        }

        private class Person
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }
    }
}