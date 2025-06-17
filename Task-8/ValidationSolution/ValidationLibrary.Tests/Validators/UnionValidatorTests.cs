using FluentAssertions;
using ValidationLibrary.Core;
using ValidationLibrary.Validators;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class UnionValidatorTests
    {
        [Fact]
        public void Union_With_Valid_String_Should_Pass()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate("valid string");

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_With_Valid_Int_Should_Pass()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate(42);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_With_Invalid_Types_Should_Fail()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate(3.14); // double, not string or int

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Value does not match any of the expected types");
        }

        [Fact]
        public void Union_With_Wrong_Value_For_Type_Should_Fail()
        {
            // Arrange
            var stringValidator = Validator.String().MinLength(5);
            var intValidator = Validator.Int().GreaterThan(100);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate("hi"); // string but too short

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Value does not match any of the expected types");
        }

        [Fact]
        public void Union_With_Null_Should_Handle_Gracefully()
        {
            // Arrange
            var stringValidator = Validator.String().NotNull();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Union_With_Multiple_Complex_Types_Should_Work()
        {
            // Arrange
            var personValidator = Validator.Object<Person>()
                .RuleFor(p => p.Name, Validator.String().NotEmpty())
                .RuleFor(p => p.Age, Validator.Int().GreaterThan(0));

            var addressValidator = Validator.Object<Address>()
                .RuleFor(a => a.Street, Validator.String().NotEmpty())
                .RuleFor(a => a.City, Validator.String().NotEmpty());

            var unionValidator = Validator.Union(personValidator, addressValidator);

            var validPerson = new Person { Name = "John", Age = 30 };

            // Act
            var result = unionValidator.Validate(validPerson);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_Or_Should_Add_Additional_Validator()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var boolValidator = Validator.Boolean();
            
            var unionValidator = Validator.Union(stringValidator, intValidator)
                .Or((IValidator)boolValidator);

            // Act
            var result1 = unionValidator.Validate("valid");
            var result2 = unionValidator.Validate(42);
            var result3 = unionValidator.Validate(true);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeTrue();
            result3.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_GetAcceptedTypes_Should_Return_All_Types()
        {
            // Arrange
            var stringValidator = Validator.String();
            var intValidator = Validator.Int();
            var boolValidator = Validator.Boolean();
            var unionValidator = Validator.Union(stringValidator, intValidator, boolValidator);

            // Act
            var acceptedTypes = unionValidator.GetAcceptedTypes();

            // Assert
            acceptedTypes.Should().HaveCount(3);
            acceptedTypes.Should().Contain(typeof(string));
            acceptedTypes.Should().Contain(typeof(int));
            acceptedTypes.Should().Contain(typeof(bool));
        }

        [Fact]
        public void Union_ValidatedType_Should_Be_Object()
        {
            // Arrange
            var stringValidator = Validator.String();
            var intValidator = Validator.Int();
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act & Assert
            unionValidator.ValidatedType.Should().Be(typeof(object));
        }

        [Fact]
        public void Union_TryValidate_With_Valid_Value_Should_Return_True()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var success = unionValidator.TryValidate("valid", out var result);

            // Assert
            success.Should().BeTrue();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_TryValidate_With_Invalid_Value_Should_Return_False()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var success = unionValidator.TryValidate(3.14, out var result);

            // Assert
            success.Should().BeFalse();
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Union_ValidateAndThrow_With_Invalid_Value_Should_Throw()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() =>
                unionValidator.ValidateAndThrow(3.14));

            exception.Message.Should().Contain("Value does not match any of the expected types");
        }

        [Fact]
        public void Generic_Union_With_Two_Types_Should_Work()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = new UnionValidator<string, int>(stringValidator, intValidator);

            // Act
            var result1 = unionValidator.Validate("valid");
            var result2 = unionValidator.Validate(42);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Generic_Union_With_Wrong_Type_Should_Fail()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = new UnionValidator<string, int>(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate(3.14);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Union_With_Empty_Validators_Should_Throw()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Validator.Union());
        }

        [Fact]
        public void Union_With_Null_Validators_Should_Throw()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Validator.Union(null!));
        }

        [Fact]
        public void Union_With_Exception_In_Validator_Should_Handle_Gracefully()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var throwingValidator = Validator.Custom<int>(_ => throw new InvalidOperationException("Test exception"));
            var unionValidator = Validator.Union(stringValidator, throwingValidator);

            // Act
            var result = unionValidator.Validate(42);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Value does not match any of the expected types");
        }

        [Fact]
        public void Union_Should_Stop_On_First_Successful_Validation()
        {
            // Arrange
            var validStringValidator = Validator.String().NotEmpty();
            var validIntValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(validStringValidator, validIntValidator);

            // Act
            var result = unionValidator.Validate("test"); // Should pass string validation first

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Union_With_Nullable_Types_Should_Handle_Null()
        {
            // Arrange
            var stringValidator = Validator.String().NotEmpty();
            var intValidator = Validator.Int().GreaterThan(0);
            var unionValidator = Validator.Union(stringValidator, intValidator);

            // Act
            var result = unionValidator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse(); // null should not be valid for either validator
        }

        [Fact]
        public void Union_With_Collection_Types_Should_Work()
        {
            // Arrange
            var arrayValidator = Validator.Array<int>().NotEmpty();
            var listValidator = Validator.List<string>().NotEmpty();
            var unionValidator = Validator.Union(arrayValidator, listValidator);

            // Act
            var result1 = unionValidator.Validate(new[] { 1, 2, 3 });
            var result2 = unionValidator.Validate(new List<string> { "a", "b" });

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeTrue();
        }

        private class Person
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        private class Address
        {
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
        }
    }
}