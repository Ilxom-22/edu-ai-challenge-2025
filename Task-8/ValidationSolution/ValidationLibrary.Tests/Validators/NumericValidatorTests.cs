using FluentAssertions;
using ValidationLibrary;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class NumericValidatorTests
    {
        [Theory]
        [InlineData(5, 3)]
        [InlineData(10, 5)]
        [InlineData(100, 50)]
        public void GreaterThan_With_Valid_Value_Should_Pass(int value, int min)
        {
            // Arrange
            var validator = Validator.Int().GreaterThan(min);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(5, 5)]
        public void GreaterThan_With_Invalid_Value_Should_Fail(int value, int min)
        {
            // Arrange
            var validator = Validator.Int().GreaterThan(min);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Value must be greater than {min}.");
        }

        [Theory]
        [InlineData(5, 3)]
        [InlineData(5, 5)]
        [InlineData(10, 10)]
        public void GreaterThanOrEqual_With_Valid_Value_Should_Pass(int value, int min)
        {
            // Arrange
            var validator = Validator.Int().GreaterThanOrEqual(min);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void GreaterThanOrEqual_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().GreaterThanOrEqual(5);

            // Act
            var result = validator.Validate(3);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be greater than or equal to 5.");
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(10, 15)]
        public void LessThan_With_Valid_Value_Should_Pass(int value, int max)
        {
            // Arrange
            var validator = Validator.Int().LessThan(max);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(5, 3)]
        [InlineData(5, 5)]
        public void LessThan_With_Invalid_Value_Should_Fail(int value, int max)
        {
            // Arrange
            var validator = Validator.Int().LessThan(max);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Value must be less than {max}.");
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(5, 5)]
        public void LessThanOrEqual_With_Valid_Value_Should_Pass(int value, int max)
        {
            // Arrange
            var validator = Validator.Int().LessThanOrEqual(max);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void LessThanOrEqual_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().LessThanOrEqual(5);

            // Act
            var result = validator.Validate(10);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be less than or equal to 5.");
        }

        [Theory]
        [InlineData(5, 3, 7)]
        [InlineData(3, 3, 7)]
        [InlineData(7, 3, 7)]
        public void Between_With_Valid_Value_Should_Pass(int value, int min, int max)
        {
            // Arrange
            var validator = Validator.Int().Between(min, max);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 3, 7)]
        [InlineData(8, 3, 7)]
        public void Between_With_Invalid_Value_Should_Fail(int value, int min, int max)
        {
            // Arrange
            var validator = Validator.Int().Between(min, max);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Value must be between {min} and {max}.");
        }

        [Fact]
        public void Equal_With_Valid_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Int().Equal(42);

            // Act
            var result = validator.Validate(42);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Equal_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().Equal(42);

            // Act
            var result = validator.Validate(41);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be equal to 42.");
        }

        [Fact]
        public void NotEqual_With_Valid_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Int().NotEqual(42);

            // Act
            var result = validator.Validate(41);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotEqual_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().NotEqual(42);

            // Act
            var result = validator.Validate(42);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must not be equal to 42.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(1000)]
        public void Positive_With_Valid_Value_Should_Pass(int value)
        {
            // Arrange
            var validator = Validator.Int().Positive();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Positive_With_Invalid_Value_Should_Fail(int value)
        {
            // Arrange
            var validator = Validator.Int().Positive();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be positive.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-42)]
        [InlineData(-1000)]
        public void Negative_With_Valid_Value_Should_Pass(int value)
        {
            // Arrange
            var validator = Validator.Int().Negative();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Negative_With_Invalid_Value_Should_Fail(int value)
        {
            // Arrange
            var validator = Validator.Int().Negative();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be negative.");
        }

        [Fact]
        public void Zero_With_Valid_Value_Should_Pass()
        {
            // Arrange
            var validator = Validator.Int().Zero();

            // Act
            var result = validator.Validate(0);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        public void Zero_With_Invalid_Value_Should_Fail(int value)
        {
            // Arrange
            var validator = Validator.Int().Zero();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be zero.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(42)]
        public void NotZero_With_Valid_Value_Should_Pass(int value)
        {
            // Arrange
            var validator = Validator.Int().NotZero();

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotZero_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().NotZero();

            // Act
            var result = validator.Validate(0);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must not be zero.");
        }

        [Theory]
        [InlineData(1, new[] { 1, 2, 3 })]
        [InlineData(2, new[] { 1, 2, 3 })]
        [InlineData(3, new[] { 1, 2, 3 })]
        public void OneOf_With_Valid_Value_Should_Pass(int value, int[] allowedValues)
        {
            // Arrange
            var validator = Validator.Int().OneOf(allowedValues);

            // Act
            var result = validator.Validate(value);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OneOf_With_Invalid_Value_Should_Fail()
        {
            // Arrange
            var validator = Validator.Int().OneOf(new[] { 1, 2, 3 });

            // Act
            var result = validator.Validate(4);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be one of: 1, 2, 3");
        }

        [Fact]
        public void Double_Validator_Should_Work_With_Decimals()
        {
            // Arrange
            var validator = Validator.Double().Between(1.0, 10.0);

            // Act
            var result = validator.Validate(5.5);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Decimal_Validator_Should_Work_With_Precise_Values()
        {
            // Arrange
            var validator = Validator.Decimal().Positive();

            // Act
            var result = validator.Validate(123.456m);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Long_Validator_Should_Work_With_Large_Values()
        {
            // Arrange
            var validator = Validator.Long().GreaterThan(1000000000L);

            // Act
            var result = validator.Validate(2000000000L);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Float_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.Float().Between(1.0f, 10.0f);

            // Act
            var result = validator.Validate(5.5f);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Short_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.Short().Between((short)1, (short)100);

            // Act
            var result = validator.Validate((short)50);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Byte_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.Byte().LessThan((byte)200);

            // Act
            var result = validator.Validate((byte)100);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Chained_Numeric_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.Int()
                .Positive()
                .LessThan(100)
                .NotEqual(42);

            // Act
            var result = validator.Validate(50);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_Numeric_Failures_Should_Return_All_Errors()
        {
            // Arrange
            var validator = Validator.Int()
                .Positive()
                .LessThan(10)
                .NotEqual(5);

            // Act
            var result = validator.Validate(5);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1); // Only the NotEqual should fail
        }
    }
} 