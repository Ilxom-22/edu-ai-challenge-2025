using FluentAssertions;
using ValidationLibrary;
using ValidationLibrary.Core;
using Xunit;
using System.Text.RegularExpressions;

namespace ValidationLibrary.Tests.Validators
{
    public class StringValidatorTests
    {
        [Fact]
        public void NotNull_With_Valid_String_Should_Pass()
        {
            // Arrange
            var validator = Validator.String().NotNull();

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotNull_With_Null_String_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().NotNull();

            // Act
            var result = validator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value cannot be null.");
        }

        [Fact]
        public void NotEmpty_With_Valid_String_Should_Pass()
        {
            // Arrange
            var validator = Validator.String().NotEmpty();

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NotEmpty_With_Null_Or_Empty_String_Should_Fail(string? input)
        {
            // Arrange
            var validator = Validator.String().NotEmpty();

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value cannot be null or empty.");
        }

        [Fact]
        public void NotWhiteSpace_With_Valid_String_Should_Pass()
        {
            // Arrange
            var validator = Validator.String().NotWhiteSpace();

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void NotWhiteSpace_With_Null_Empty_Or_Whitespace_Should_Fail(string? input)
        {
            // Arrange
            var validator = Validator.String().NotWhiteSpace();

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value cannot be null, empty, or whitespace.");
        }

        [Theory]
        [InlineData("test", 4)]
        [InlineData("hello", 5)]
        [InlineData("a", 1)]
        public void MinLength_With_Valid_Length_Should_Pass(string input, int minLength)
        {
            // Arrange
            var validator = Validator.String().MinLength(minLength);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MinLength_With_Too_Short_String_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().MinLength(5);

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be at least 5 characters long.");
        }

        [Theory]
        [InlineData("test", 4)]
        [InlineData("hello", 5)]
        [InlineData("a", 10)]
        public void MaxLength_With_Valid_Length_Should_Pass(string input, int maxLength)
        {
            // Arrange
            var validator = Validator.String().MaxLength(maxLength);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MaxLength_With_Too_Long_String_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().MaxLength(3);

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be at most 3 characters long.");
        }

        [Fact]
        public void Length_With_Exact_Length_Should_Pass()
        {
            // Arrange
            var validator = Validator.String().Length(4);

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Length_With_Wrong_Length_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().Length(5);

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be exactly 5 characters long.");
        }

        [Theory]
        [InlineData("test", 3, 5)]
        [InlineData("hello", 3, 5)]
        [InlineData("hi", 2, 5)]
        public void Length_Range_With_Valid_Length_Should_Pass(string input, int min, int max)
        {
            // Arrange
            var validator = Validator.String().Length(min, max);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Length_Range_With_Invalid_Length_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().Length(5, 10);

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be between 5 and 10 characters long.");
        }

        [Theory]
        [InlineData("test123", @"^\w+\d+$")]
        [InlineData("hello", @"^[a-z]+$")]
        public void Matches_With_Valid_Pattern_Should_Pass(string input, string pattern)
        {
            // Arrange
            var validator = Validator.String().Matches(pattern);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Matches_With_Invalid_Pattern_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().Matches(@"^\d+$");

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("must match the pattern");
        }

        [Fact]
        public void Matches_With_Regex_Object_Should_Work()
        {
            // Arrange
            var regex = new Regex(@"^\d+$");
            var validator = Validator.String().Matches(regex);

            // Act
            var result = validator.Validate("123");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("hello world", "world")]
        [InlineData("test string", "test")]
        [InlineData("example", "amp")]
        public void Contains_With_Valid_Substring_Should_Pass(string input, string substring)
        {
            // Arrange
            var validator = Validator.String().Contains(substring);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Contains_With_Missing_Substring_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().Contains("xyz");

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must contain: xyz");
        }

        [Theory]
        [InlineData("hello world", "hello")]
        [InlineData("test string", "test")]
        public void StartsWith_With_Valid_Prefix_Should_Pass(string input, string prefix)
        {
            // Arrange
            var validator = Validator.String().StartsWith(prefix);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void StartsWith_With_Invalid_Prefix_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().StartsWith("xyz");

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must start with: xyz");
        }

        [Theory]
        [InlineData("hello world", "world")]
        [InlineData("test string", "string")]
        public void EndsWith_With_Valid_Suffix_Should_Pass(string input, string suffix)
        {
            // Arrange
            var validator = Validator.String().EndsWith(suffix);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void EndsWith_With_Invalid_Suffix_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().EndsWith("xyz");

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must end with: xyz");
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.email+tag@domain.co.uk")]
        public void Email_With_Valid_Email_Should_Pass(string email)
        {
            // Arrange
            var validator = Validator.String().Email();

            // Act
            var result = validator.Validate(email);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        [InlineData("user@domain")]
        public void Email_With_Invalid_Email_Should_Fail(string email)
        {
            // Arrange
            var validator = Validator.String().Email();

            // Act
            var result = validator.Validate(email);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be a valid email address.");
        }

        [Theory]
        [InlineData("https://www.example.com")]
        [InlineData("http://domain.com")]
        [InlineData("ftp://files.example.com")]
        public void Url_With_Valid_Url_Should_Pass(string url)
        {
            // Arrange
            var validator = Validator.String().Url();

            // Act
            var result = validator.Validate(url);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("://invalid")]
        [InlineData("www.example.com")]
        public void Url_With_Invalid_Url_Should_Fail(string url)
        {
            // Arrange
            var validator = Validator.String().Url();

            // Act
            var result = validator.Validate(url);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be a valid URL.");
        }

        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        public void Guid_With_Valid_Guid_Should_Pass(string guid)
        {
            // Arrange
            var validator = Validator.String().Guid();

            // Act
            var result = validator.Validate(guid);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("550e8400-e29b-41d4-a716")]
        public void Guid_With_Invalid_Guid_Should_Fail(string guid)
        {
            // Arrange
            var validator = Validator.String().Guid();

            // Act
            var result = validator.Validate(guid);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be a valid GUID.");
        }

        [Theory]
        [InlineData("option1", new[] { "option1", "option2", "option3" })]
        [InlineData("option2", new[] { "option1", "option2", "option3" })]
        public void OneOf_With_Valid_Option_Should_Pass(string input, string[] options)
        {
            // Arrange
            var validator = Validator.String().OneOf(options);

            // Act
            var result = validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OneOf_With_Invalid_Option_Should_Fail()
        {
            // Arrange
            var validator = Validator.String().OneOf(new[] { "option1", "option2" });

            // Act
            var result = validator.Validate("option3");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Value must be one of: option1, option2");
        }

        [Fact]
        public void Chained_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.String()
                .NotNull()
                .NotEmpty()
                .MinLength(5)
                .MaxLength(20)
                .Contains("test");

            // Act
            var result = validator.Validate("test123");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_Failures_Should_Return_All_Errors()
        {
            // Arrange
            var validator = Validator.String()
                .NotEmpty()
                .MinLength(10)
                .Contains("xyz");

            // Act
            var result = validator.Validate("test");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void ValidateAndThrow_With_Valid_Input_Should_Return_Value()
        {
            // Arrange
            var validator = Validator.String().NotEmpty();
            const string input = "test";

            // Act
            var result = validator.ValidateAndThrow(input);

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void ValidateAndThrow_With_Invalid_Input_Should_Throw_Exception()
        {
            // Arrange
            var validator = Validator.String().NotEmpty();

            // Act & Assert
            var action = () => validator.ValidateAndThrow("");
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void TryValidate_With_Valid_Input_Should_Return_True()
        {
            // Arrange
            var validator = Validator.String().NotEmpty();

            // Act
            var success = validator.TryValidate("test", out var result);

            // Assert
            success.Should().BeTrue();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void TryValidate_With_Invalid_Input_Should_Return_False()
        {
            // Arrange
            var validator = Validator.String().NotEmpty();

            // Act
            var success = validator.TryValidate("", out var result);

            // Assert
            success.Should().BeFalse();
            result.IsValid.Should().BeFalse();
        }
    }
} 