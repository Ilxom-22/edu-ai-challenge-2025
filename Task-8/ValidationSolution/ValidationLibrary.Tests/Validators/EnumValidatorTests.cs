using FluentAssertions;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class EnumValidatorTests
    {
        public enum TestEnum
        {
            None = 0,
            First = 1,
            Second = 2,
            Third = 3
        }

        [Flags]
        public enum TestFlags
        {
            None = 0,
            Read = 1,
            Write = 2,
            Execute = 4,
            ReadWrite = Read | Write,
            All = Read | Write | Execute
        }

        public enum EmptyEnum
        {
        }

        [Fact]
        public void IsDefined_With_Valid_Enum_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsDefined_With_Invalid_Enum_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();
            var invalidValue = (TestEnum)999;

            // Act
            var result = validator.Validate(invalidValue);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be a valid TestEnum enum value.");
        }

        [Fact]
        public void IsDefined_With_Custom_Message_Should_Use_Custom_Message()
        {
            // Arrange
            var customMessage = "Invalid enum value provided";
            var validator = Validator.Enum<TestEnum>().IsDefined(customMessage);
            var invalidValue = (TestEnum)999;

            // Act
            var result = validator.Validate(invalidValue);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be(customMessage);
        }

        [Fact]
        public void OneOf_With_Allowed_Value_Should_Pass()
        {
            // Arrange
            var allowedValues = new[] { TestEnum.First, TestEnum.Second };
            var validator = Validator.Enum<TestEnum>().OneOf(allowedValues);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OneOf_With_Disallowed_Value_Should_Fail()
        {
            // Arrange
            var allowedValues = new[] { TestEnum.First, TestEnum.Second };
            var validator = Validator.Enum<TestEnum>().OneOf(allowedValues);

            // Act
            var result = validator.Validate(TestEnum.Third);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be one of: First, Second");
        }

        [Fact]
        public void OneOf_With_Single_Allowed_Value_Should_Pass()
        {
            // Arrange
            var allowedValues = new[] { TestEnum.First };
            var validator = Validator.Enum<TestEnum>().OneOf(allowedValues);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OneOf_With_Empty_Array_Should_Always_Fail()
        {
            // Arrange
            var allowedValues = new TestEnum[0];
            var validator = Validator.Enum<TestEnum>().OneOf(allowedValues);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void NotOneOf_With_Forbidden_Value_Should_Fail()
        {
            // Arrange
            var forbiddenValues = new[] { TestEnum.First, TestEnum.Second };
            var validator = Validator.Enum<TestEnum>().NotOneOf(forbiddenValues);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must not be one of: First, Second");
        }

        [Fact]
        public void NotOneOf_With_Allowed_Value_Should_Pass()
        {
            // Arrange
            var forbiddenValues = new[] { TestEnum.First, TestEnum.Second };
            var validator = Validator.Enum<TestEnum>().NotOneOf(forbiddenValues);

            // Act
            var result = validator.Validate(TestEnum.Third);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotOneOf_With_Empty_Array_Should_Always_Pass()
        {
            // Arrange
            var forbiddenValues = new TestEnum[0];
            var validator = Validator.Enum<TestEnum>().NotOneOf(forbiddenValues);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Equal_With_Same_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().Equal(TestEnum.First);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Equal_With_Different_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().Equal(TestEnum.First);

            // Act
            var result = validator.Validate(TestEnum.Second);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must equal First.");
        }

        [Fact]
        public void NotEqual_With_Different_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().NotEqual(TestEnum.First);

            // Act
            var result = validator.Validate(TestEnum.Second);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotEqual_With_Same_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().NotEqual(TestEnum.First);

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must not equal First.");
        }

        [Fact]
        public void HasFlag_With_Set_Flag_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().HasFlag(TestFlags.Read);

            // Act
            var result = validator.Validate(TestFlags.ReadWrite);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void HasFlag_With_Unset_Flag_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().HasFlag(TestFlags.Execute);

            // Act
            var result = validator.Validate(TestFlags.ReadWrite);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must have flag Execute set.");
        }

        [Fact]
        public void HasFlag_With_Exact_Match_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().HasFlag(TestFlags.Read);

            // Act
            var result = validator.Validate(TestFlags.Read);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void HasFlag_With_None_Flag_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().HasFlag(TestFlags.None);

            // Act
            var result = validator.Validate(TestFlags.Read);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveFlag_With_Unset_Flag_Should_Pass()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().DoesNotHaveFlag(TestFlags.Execute);

            // Act
            var result = validator.Validate(TestFlags.ReadWrite);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveFlag_With_Set_Flag_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().DoesNotHaveFlag(TestFlags.Read);

            // Act
            var result = validator.Validate(TestFlags.ReadWrite);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must not have flag Read set.");
        }

        [Fact]
        public void DoesNotHaveFlag_With_None_Flag_Should_Fail()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().DoesNotHaveFlag(TestFlags.None);

            // Act
            var result = validator.Validate(TestFlags.None);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Chained_Enum_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>()
                .IsDefined()
                .NotEqual(TestEnum.None)
                .OneOf(new[] { TestEnum.First, TestEnum.Second, TestEnum.Third });

            // Act
            var result = validator.Validate(TestEnum.First);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_Enum_Failures_Should_Return_All_Errors()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>()
                .Equal(TestEnum.First)
                .NotEqual(TestEnum.Second)
                .OneOf(new[] { TestEnum.Third });

            // Act
            var result = validator.Validate(TestEnum.Second);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3); // Equal fails, NotEqual fails, OneOf fails
        }

        [Fact]
        public void Chained_Flag_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>()
                .HasFlag(TestFlags.Read)
                .DoesNotHaveFlag(TestFlags.Execute)
                .NotEqual(TestFlags.None);

            // Act
            var result = validator.Validate(TestFlags.ReadWrite);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void TryValidate_With_Valid_Enum_Should_Return_True()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();

            // Act
            var success = validator.TryValidate(TestEnum.First, out var result);

            // Assert
            success.Should().BeTrue();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void TryValidate_With_Invalid_Enum_Should_Return_False()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();
            var invalidValue = (TestEnum)999;

            // Act
            var success = validator.TryValidate(invalidValue, out var result);

            // Assert
            success.Should().BeFalse();
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAndThrow_With_Invalid_Enum_Should_Throw_Exception()
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();
            var invalidValue = (TestEnum)999;

            // Act & Assert
            var exception = Assert.Throws<ValidationLibrary.Core.ValidationException>(() =>
                validator.ValidateAndThrow(invalidValue));

            exception.Message.Should().Contain("TestEnum enum value");
        }

        [Theory]
        [InlineData(TestEnum.None)]
        [InlineData(TestEnum.First)]
        [InlineData(TestEnum.Second)]
        [InlineData(TestEnum.Third)]
        public void IsDefined_With_All_Valid_Enum_Values_Should_Pass(TestEnum value)
        {
            // Arrange
            var validator = Validator.Enum<TestEnum>().IsDefined();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(TestFlags.None)]
        [InlineData(TestFlags.Read)]
        [InlineData(TestFlags.Write)]
        [InlineData(TestFlags.Execute)]
        [InlineData(TestFlags.ReadWrite)]
        [InlineData(TestFlags.All)]
        public void IsDefined_With_All_Valid_Flag_Values_Should_Pass(TestFlags value)
        {
            // Arrange
            var validator = Validator.Enum<TestFlags>().IsDefined();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}