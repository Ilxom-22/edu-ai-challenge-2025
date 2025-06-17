using FluentAssertions;
using ValidationLibrary;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class ObjectValidatorTests
    {
        private class TestUser
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int Age { get; set; }
            public DateTime DateOfBirth { get; set; }
            public bool IsActive { get; set; }
            public string[] Tags { get; set; } = Array.Empty<string>();
        }

        [Fact]
        public void NotNull_With_Valid_Object_Should_Pass()
        {
            // Arrange
            var validator = Validator.Object<TestUser>().NotNull();
            var user = new TestUser();

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotNull_With_Null_Object_Should_Fail()
        {
            // Arrange
            var validator = Validator.Object<TestUser>().NotNull();

            // Act
            var result = validator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Object cannot be null.");
        }

        [Fact]
        public void RuleFor_With_Valid_Property_Should_Pass()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Name, Validator.String().NotEmpty())
                .RuleFor(u => u.Age, Validator.Int().Positive());

            var user = new TestUser
            {
                Name = "John Doe",
                Age = 25
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_With_Invalid_Property_Should_Fail_With_Property_Path()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Name, Validator.String().NotEmpty())
                .RuleFor(u => u.Age, Validator.Int().Positive());

            var user = new TestUser
            {
                Name = "",
                Age = -5
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.PropertyPath == "Name");
            result.Errors.Should().Contain(e => e.PropertyPath == "Age");
        }

        [Fact]
        public void RuleFor_With_Predicate_Should_Work()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Name, name => !string.IsNullOrEmpty(name), "Name is required");

            var user = new TestUser { Name = "Valid Name" };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RuleFor_With_Predicate_Failure_Should_Include_Property_Path()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Name, name => !string.IsNullOrEmpty(name), "Name is required");

            var user = new TestUser { Name = "" };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().PropertyPath.Should().Be("Name");
        }

        [Fact]
        public void Must_With_Valid_Condition_Should_Pass()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .Must(u => u.Age >= 18, "User must be at least 18 years old");

            var user = new TestUser { Age = 25 };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Must_With_Invalid_Condition_Should_Fail()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .Must(u => u.Age >= 18, "User must be at least 18 years old");

            var user = new TestUser { Age = 16 };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("User must be at least 18 years old");
        }

        [Fact]
        public void When_With_True_Condition_Should_Apply_Validation()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .When(u => u.IsActive, v => v
                    .RuleFor(u => u.Email, Validator.String().Email()));

            var user = new TestUser
            {
                IsActive = true,
                Email = "invalid-email"
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyPath == "Email");
        }

        [Fact]
        public void When_With_False_Condition_Should_Skip_Validation()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .When(u => u.IsActive, v => v
                    .RuleFor(u => u.Email, Validator.String().Email()));

            var user = new TestUser
            {
                IsActive = false,
                Email = "invalid-email"
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Unless_With_False_Condition_Should_Apply_Validation()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .Unless(u => u.IsActive, v => v
                    .RuleFor(u => u.Name, Validator.String().NotEmpty()));

            var user = new TestUser
            {
                IsActive = false,
                Name = ""
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyPath == "Name");
        }

        [Fact]
        public void Unless_With_True_Condition_Should_Skip_Validation()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .Unless(u => u.IsActive, v => v
                    .RuleFor(u => u.Name, Validator.String().NotEmpty()));

            var user = new TestUser
            {
                IsActive = true,
                Name = ""
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Complex_Validation_With_Multiple_Rules_Should_Work()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .NotNull()
                .RuleFor(u => u.Name, Validator.String().NotEmpty().MinLength(2).MaxLength(50))
                .RuleFor(u => u.Email, Validator.String().Email())
                .RuleFor(u => u.Age, Validator.Int().Between(0, 150))
                .RuleFor(u => u.DateOfBirth, Validator.DateTime().InThePast())
                .RuleFor(u => u.Tags, Validator.Array<string>().MaxCount(5))
                .Must(u => u.Age >= 18 || u.Name.StartsWith("Minor"), "Adults must be 18+ or marked as minor");

            var user = new TestUser
            {
                Name = "John Doe",
                Email = "john@example.com",
                Age = 25,
                DateOfBirth = DateTime.Now.AddYears(-25),
                Tags = new[] { "developer", "senior" }
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Nested_Object_Validation_Should_Include_Property_Paths()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Tags, Validator.Array<string>()
                    .ForEach(Validator.String().NotEmpty().MinLength(2)));

            var user = new TestUser
            {
                Tags = new[] { "valid", "x", "" }
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            // Check that nested property paths are correctly formed
            result.Errors.Should().Contain(e => e.PropertyPath.StartsWith("Tags"));
        }

        [Fact]
        public void Object_Validator_With_Null_Should_Skip_Property_Validation()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .RuleFor(u => u.Name, Validator.String().NotEmpty());

            // Act
            var result = validator.Validate(null);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_Conditional_Validations_Should_Work_Together()
        {
            // Arrange
            var validator = Validator.Object<TestUser>()
                .When(u => u.Age >= 18, v => v
                    .RuleFor(u => u.Email, Validator.String().Email()))
                .Unless(u => u.IsActive, v => v
                    .RuleFor(u => u.Name, Validator.String().StartsWith("Inactive")));

            var user = new TestUser
            {
                Age = 20,
                IsActive = false,
                Email = "invalid-email",
                Name = "Active User"
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }
    }
} 