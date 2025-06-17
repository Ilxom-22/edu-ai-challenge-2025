using FluentAssertions;
using ValidationLibrary;
using ValidationLibrary.Core;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class CollectionValidatorTests
    {
        [Fact]
        public void Array_NotNull_With_Valid_Array_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<string>().NotNull();
            var array = new[] { "test" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Array_NotNull_With_Null_Array_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<string>().NotNull();

            // Act
            var result = validator.Validate(null);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection cannot be null.");
        }

        [Fact]
        public void Array_NotEmpty_With_Valid_Array_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<string>().NotEmpty();
            var array = new[] { "test" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Array_NotEmpty_With_Empty_Array_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<string>().NotEmpty();
            var array = Array.Empty<string>();

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection cannot be empty.");
        }

        [Theory]
        [InlineData(3, 2)]
        [InlineData(5, 3)]
        [InlineData(1, 1)]
        public void MinCount_With_Valid_Count_Should_Pass(int count, int minCount)
        {
            // Arrange
            var validator = Validator.Array<int>().MinCount(minCount);
            var array = new int[count];

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MinCount_With_Invalid_Count_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().MinCount(3);
            var array = new[] { 1, 2 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain at least 3 items.");
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(1, 5)]
        public void MaxCount_With_Valid_Count_Should_Pass(int count, int maxCount)
        {
            // Arrange
            var validator = Validator.Array<int>().MaxCount(maxCount);
            var array = new int[count];

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MaxCount_With_Invalid_Count_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().MaxCount(2);
            var array = new[] { 1, 2, 3 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain at most 2 items.");
        }

        [Fact]
        public void Count_With_Exact_Count_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<int>().Count(3);
            var array = new[] { 1, 2, 3 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Count_With_Wrong_Count_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().Count(3);
            var array = new[] { 1, 2 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain exactly 3 items.");
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3 }, 2, 4)]
        [InlineData(new[] { 1, 2 }, 2, 4)]
        [InlineData(new[] { 1, 2, 3, 4 }, 2, 4)]
        public void Count_Range_With_Valid_Count_Should_Pass(int[] array, int minCount, int maxCount)
        {
            // Arrange
            var validator = Validator.Array<int>().Count(minCount, maxCount);

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Count_Range_With_Invalid_Count_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().Count(3, 5);
            var array = new[] { 1, 2 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain between 3 and 5 items.");
        }

        [Fact]
        public void Unique_With_Unique_Items_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<int>().Unique();
            var array = new[] { 1, 2, 3, 4 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Unique_With_Duplicate_Items_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().Unique();
            var array = new[] { 1, 2, 2, 3 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain unique items.");
        }

        [Fact]
        public void Contains_With_Present_Item_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<string>().Contains("test");
            var array = new[] { "hello", "test", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Contains_With_Missing_Item_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<string>().Contains("missing");
            var array = new[] { "hello", "test", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must contain: missing");
        }

        [Fact]
        public void DoesNotContain_With_Absent_Item_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<string>().DoesNotContain("forbidden");
            var array = new[] { "hello", "test", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DoesNotContain_With_Present_Item_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<string>().DoesNotContain("forbidden");
            var array = new[] { "hello", "forbidden", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Collection must not contain: forbidden");
        }

        [Fact]
        public void ForEach_With_Valid_Items_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<string>().ForEach(Validator.String().NotEmpty());
            var array = new[] { "hello", "test", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ForEach_With_Invalid_Items_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<string>().ForEach(Validator.String().NotEmpty());
            var array = new[] { "hello", "", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void All_With_Matching_Predicate_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<int>().All(x => x > 0);
            var array = new[] { 1, 2, 3, 4 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void All_With_Non_Matching_Predicate_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().All(x => x > 0);
            var array = new[] { 1, -2, 3, 4 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("All items in the collection must satisfy the condition.");
        }

        [Fact]
        public void Any_With_Matching_Predicate_Should_Pass()
        {
            // Arrange
            var validator = Validator.Array<int>().Any(x => x > 5);
            var array = new[] { 1, 2, 10, 4 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Any_With_Non_Matching_Predicate_Should_Fail()
        {
            // Arrange
            var validator = Validator.Array<int>().Any(x => x > 10);
            var array = new[] { 1, 2, 3, 4 };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("At least one item in the collection must satisfy the condition.");
        }

        [Fact]
        public void List_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.List<int>().NotEmpty().MinCount(2);
            var list = new List<int> { 1, 2, 3 };

            // Act
            var result = validator.Validate(list);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Enumerable_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.Enumerable<string>().NotNull().Count(3);
            IEnumerable<string> enumerable = new[] { "a", "b", "c" };

            // Act
            var result = validator.Validate(enumerable);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Collection_Validator_Should_Work()
        {
            // Arrange
            var validator = Validator.Collection<int>().NotEmpty().MaxCount(5);
            ICollection<int> collection = new List<int> { 1, 2, 3 };

            // Act
            var result = validator.Validate(collection);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Chained_Collection_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.Array<string>()
                .NotNull()
                .NotEmpty()
                .MinCount(2)
                .MaxCount(5)
                .Unique()
                .ForEach(Validator.String().NotEmpty());

            var array = new[] { "hello", "world", "test" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_Collection_Failures_Should_Return_All_Errors()
        {
            // Arrange
            var validator = Validator.Array<string>()
                .NotEmpty()
                .MinCount(5)
                .Contains("required");

            var array = new[] { "hello", "world" };

            // Act
            var result = validator.Validate(array);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }
    }
} 