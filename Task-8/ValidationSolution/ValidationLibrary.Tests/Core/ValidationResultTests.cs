using FluentAssertions;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Core
{
    public class ValidationResultTests
    {
        [Fact]
        public void Success_Should_Return_Valid_Result()
        {
            // Act
            var result = ValidationResult.Success();

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Failure_With_Single_Message_Should_Return_Invalid_Result()
        {
            // Arrange
            const string errorMessage = "Test error";

            // Act
            var result = ValidationResult.Failure(errorMessage);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.ErrorMessage.Should().Be(errorMessage);
            result.Errors.First().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void Failure_With_Message_And_PropertyName_Should_Set_Property()
        {
            // Arrange
            const string errorMessage = "Test error";
            const string propertyName = "TestProperty";

            // Act
            var result = ValidationResult.Failure(errorMessage, propertyName);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().PropertyName.Should().Be(propertyName);
        }

        [Fact]
        public void Failure_With_Multiple_Errors_Should_Include_All_Errors()
        {
            // Arrange
            var errors = new[]
            {
                new ValidationError("Error 1"),
                new ValidationError("Error 2"),
                new ValidationError("Error 3")
            };

            // Act
            var result = ValidationResult.Failure(errors);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
            result.ErrorMessage.Should().Be("Error 1");
        }

        [Fact]
        public void Failure_With_Multiple_Messages_Should_Create_Errors()
        {
            // Arrange
            var messages = new[] { "Error 1", "Error 2", "Error 3" };
            const string propertyName = "TestProperty";

            // Act
            var result = ValidationResult.Failure(messages, propertyName);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
            result.Errors.All(e => e.PropertyName == propertyName).Should().BeTrue();
        }

        [Fact]
        public void Combine_With_All_Valid_Results_Should_Return_Valid()
        {
            // Arrange
            var results = new[]
            {
                ValidationResult.Success(),
                ValidationResult.Success(),
                ValidationResult.Success()
            };

            // Act
            var combined = ValidationResult.Combine(results);

            // Assert
            combined.IsValid.Should().BeTrue();
            combined.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Combine_With_Mixed_Results_Should_Return_Invalid_With_All_Errors()
        {
            // Arrange
            var results = new[]
            {
                ValidationResult.Success(),
                ValidationResult.Failure("Error 1"),
                ValidationResult.Failure("Error 2")
            };

            // Act
            var combined = ValidationResult.Combine(results);

            // Assert
            combined.IsValid.Should().BeFalse();
            combined.Errors.Should().HaveCount(2);
            combined.Errors.Select(e => e.Message).Should().Contain(new[] { "Error 1", "Error 2" });
        }

        [Fact]
        public void And_Should_Combine_Two_Results()
        {
            // Arrange
            var result1 = ValidationResult.Failure("Error 1");
            var result2 = ValidationResult.Failure("Error 2");

            // Act
            var combined = result1.And(result2);

            // Assert
            combined.IsValid.Should().BeFalse();
            combined.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void ToString_With_Valid_Result_Should_Return_Success_Message()
        {
            // Arrange
            var result = ValidationResult.Success();

            // Act
            var toString = result.ToString();

            // Assert
            toString.Should().Be("Validation succeeded");
        }

        [Fact]
        public void ToString_With_Invalid_Result_Should_Return_Error_Messages()
        {
            // Arrange
            var result = ValidationResult.Failure(new[] { "Error 1", "Error 2" });

            // Act
            var toString = result.ToString();

            // Assert
            toString.Should().Be("Validation failed: Error 1; Error 2");
        }

        [Fact]
        public void Failure_With_Empty_Errors_Should_Return_Valid_Result()
        {
            // Arrange
            var emptyErrors = Array.Empty<ValidationError>();

            // Act
            var result = ValidationResult.Failure(emptyErrors);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Combine_With_Empty_Array_Should_Return_Valid_Result()
        {
            // Act
            var result = ValidationResult.Combine();

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
} 