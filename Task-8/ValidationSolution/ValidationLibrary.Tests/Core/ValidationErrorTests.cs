using FluentAssertions;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Core
{
    public class ValidationErrorTests
    {
        [Fact]
        public void Constructor_With_Message_Should_Set_Properties()
        {
            // Arrange
            const string message = "Test error message";

            // Act
            var error = new ValidationError(message);

            // Assert
            error.Message.Should().Be(message);
            error.PropertyName.Should().BeNull();
            error.PropertyPath.Should().BeNull();
            error.ErrorCode.Should().BeNull();
        }

        [Fact]
        public void Constructor_With_Message_And_PropertyName_Should_Set_Properties()
        {
            // Arrange
            const string message = "Test error message";
            const string propertyName = "TestProperty";

            // Act
            var error = new ValidationError(message, propertyName);

            // Assert
            error.Message.Should().Be(message);
            error.PropertyName.Should().Be(propertyName);
            error.PropertyPath.Should().Be(propertyName);
            error.ErrorCode.Should().BeNull();
        }

        [Fact]
        public void Constructor_With_All_Parameters_Should_Set_All_Properties()
        {
            // Arrange
            const string message = "Test error message";
            const string propertyName = "TestProperty";
            const string propertyPath = "Root.TestProperty";
            const string errorCode = "TEST_ERROR";

            // Act
            var error = new ValidationError(message, propertyName, propertyPath, errorCode);

            // Assert
            error.Message.Should().Be(message);
            error.PropertyName.Should().Be(propertyName);
            error.PropertyPath.Should().Be(propertyPath);
            error.ErrorCode.Should().Be(errorCode);
        }

        [Fact]
        public void WithPath_Should_Create_New_Error_With_Updated_Path()
        {
            // Arrange
            var originalError = new ValidationError("Test message", "Property");
            const string newPath = "Root.Property";

            // Act
            var updatedError = originalError.WithPath(newPath);

            // Assert
            updatedError.Should().NotBeSameAs(originalError);
            updatedError.Message.Should().Be(originalError.Message);
            updatedError.PropertyName.Should().Be(originalError.PropertyName);
            updatedError.PropertyPath.Should().Be(newPath);
            updatedError.ErrorCode.Should().Be(originalError.ErrorCode);
        }

        [Fact]
        public void WithCode_Should_Create_New_Error_With_Updated_ErrorCode()
        {
            // Arrange
            var originalError = new ValidationError("Test message", "Property");
            const string errorCode = "NEW_ERROR_CODE";

            // Act
            var updatedError = originalError.WithCode(errorCode);

            // Assert
            updatedError.Should().NotBeSameAs(originalError);
            updatedError.Message.Should().Be(originalError.Message);
            updatedError.PropertyName.Should().Be(originalError.PropertyName);
            updatedError.PropertyPath.Should().Be(originalError.PropertyPath);
            updatedError.ErrorCode.Should().Be(errorCode);
        }

        [Fact]
        public void ToString_Should_Return_Formatted_String()
        {
            // Arrange
            var error = new ValidationError("Test message", "TestProperty", "Root.TestProperty", "TEST_CODE");

            // Act
            var result = error.ToString();

            // Assert
            result.Should().Be("[TEST_CODE] Root.TestProperty: Test message");
        }

        [Fact]
        public void ToString_Without_PropertyPath_Should_Use_Message_Only()
        {
            // Arrange
            var error = new ValidationError("Test message");

            // Act
            var result = error.ToString();

            // Assert
            result.Should().Be("Test message");
        }

        [Fact]
        public void ToString_Without_ErrorCode_Should_Exclude_Code()
        {
            // Arrange
            var error = new ValidationError("Test message", "TestProperty", "Root.TestProperty");

            // Act
            var result = error.ToString();

            // Assert
            result.Should().Be("Root.TestProperty: Test message");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_With_Null_Or_Empty_Message_Should_Handle_Gracefully(string? message)
        {
            // Act
            var error = new ValidationError(message!);

            // Assert
            error.Message.Should().Be(message);
        }

        [Fact]
        public void Two_Errors_With_Same_Properties_Should_Be_Equal()
        {
            // Arrange
            var error1 = new ValidationError("Message", "Property", "Path", "Code");
            var error2 = new ValidationError("Message", "Property", "Path", "Code");

            // Act & Assert
            error1.Equals(error2).Should().BeTrue();
            error1.GetHashCode().Should().Be(error2.GetHashCode());
        }

        [Fact]
        public void Two_Errors_With_Different_Properties_Should_Not_Be_Equal()
        {
            // Arrange
            var error1 = new ValidationError("Message", "Property", "Path", "Code");
            var error2 = new ValidationError("Different", "Property", "Path", "Code");

            // Act & Assert
            error1.Equals(error2).Should().BeFalse();
        }
    }
} 