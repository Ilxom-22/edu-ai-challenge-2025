using FluentAssertions;
using ValidationLibrary;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class BooleanValidatorTests
    {
        [Fact]
        public void IsTrue_With_True_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Boolean().IsTrue();

            // Act
            var result = validator.Validate(true);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsTrue_With_False_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Boolean().IsTrue();

            // Act
            var result = validator.Validate(false);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be true.");
        }

        [Fact]
        public void IsFalse_With_False_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Boolean().IsFalse();

            // Act
            var result = validator.Validate(false);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsFalse_With_True_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Boolean().IsFalse();

            // Act
            var result = validator.Validate(true);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be false.");
        }

        [Fact]
        public void Equal_With_Matching_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Boolean().Equal(true);

            // Act
            var result = validator.Validate(true);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Equal_With_Non_Matching_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Boolean().Equal(true);

            // Act
            var result = validator.Validate(false);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be equal to True.");
        }

        [Fact]
        public void Chained_Boolean_Validations_Should_Work()
        {
            // Arrange
            var validator = Validator.Boolean().IsTrue().Equal(true);

            // Act
            var result = validator.Validate(true);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Custom_Error_Message_Should_Be_Used()
        {
            // Arrange
            var validator = Validator.Boolean().IsTrue("Custom error message");

            // Act
            var result = validator.Validate(false);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Custom error message");
        }
    }
} 