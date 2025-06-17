using FluentAssertions;
using Xunit;

namespace ValidationLibrary.Tests.Validators
{
    public class DateTimeValidatorTests
    {
        private readonly DateTime _fixedDate = new DateTime(2024, 6, 15, 10, 30, 0);
        private readonly DateTime _earlierDate = new DateTime(2024, 6, 10, 8, 0, 0);
        private readonly DateTime _laterDate = new DateTime(2024, 6, 20, 15, 45, 0);

        [Fact]
        public void After_With_Later_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().After(_earlierDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void After_With_Earlier_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().After(_laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Date must be after {_laterDate:yyyy-MM-dd HH:mm:ss}.");
        }

        [Fact]
        public void After_With_Same_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().After(_fixedDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void OnOrAfter_With_Later_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrAfter(_earlierDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OnOrAfter_With_Same_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrAfter(_fixedDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OnOrAfter_With_Earlier_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrAfter(_laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Date must be on or after {_laterDate:yyyy-MM-dd HH:mm:ss}.");
        }

        [Fact]
        public void Before_With_Earlier_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().Before(_laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Before_With_Later_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().Before(_earlierDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Date must be before {_earlierDate:yyyy-MM-dd HH:mm:ss}.");
        }

        [Fact]
        public void Before_With_Same_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().Before(_fixedDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void OnOrBefore_With_Earlier_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrBefore(_laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OnOrBefore_With_Same_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrBefore(_fixedDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OnOrBefore_With_Later_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().OnOrBefore(_earlierDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Date must be on or before {_earlierDate:yyyy-MM-dd HH:mm:ss}.");
        }

        [Fact]
        public void Between_With_Valid_Range_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().Between(_earlierDate, _laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Between_With_Date_At_Min_Boundary_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().Between(_fixedDate, _laterDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Between_With_Date_At_Max_Boundary_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().Between(_earlierDate, _fixedDate);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Between_With_Date_Outside_Range_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().Between(_earlierDate, new DateTime(2024, 6, 12));

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Date must be between");
        }

        [Fact]
        public void InThePast_With_Past_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().InThePast();
            var pastDate = DateTime.Now.AddDays(-1);

            // Act
            var result = validator.Validate(pastDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void InThePast_With_Future_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().InThePast();
            var futureDate = DateTime.Now.AddDays(1);

            // Act
            var result = validator.Validate(futureDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be in the past.");
        }

        [Fact]
        public void InTheFuture_With_Future_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().InTheFuture();
            var futureDate = DateTime.Now.AddDays(1);

            // Act
            var result = validator.Validate(futureDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void InTheFuture_With_Past_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().InTheFuture();
            var pastDate = DateTime.Now.AddDays(-1);

            // Act
            var result = validator.Validate(pastDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be in the future.");
        }

        [Fact]
        public void IsToday_With_Today_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().IsToday();
            var todayDate = DateTime.Today.AddHours(10);

            // Act
            var result = validator.Validate(todayDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsToday_With_Yesterday_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().IsToday();
            var yesterdayDate = DateTime.Today.AddDays(-1);

            // Act
            var result = validator.Validate(yesterdayDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be today.");
        }

        [Fact]
        public void IsToday_With_Tomorrow_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().IsToday();
            var tomorrowDate = DateTime.Today.AddDays(1);

            // Act
            var result = validator.Validate(tomorrowDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be today.");
        }

        [Fact]
        public void NotDefault_With_Non_Default_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().NotDefault();

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotDefault_With_Default_Date_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().NotDefault();

            // Act
            var result = validator.Validate(default(DateTime));

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date cannot be the default value.");
        }

        [Fact]
        public void WithinDaysFromToday_With_Valid_Date_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().WithinDaysFromToday(7);
            var validDate = DateTime.Today.AddDays(5);

            // Act
            var result = validator.Validate(validDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithinDaysFromToday_With_Today_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().WithinDaysFromToday(7);

            // Act
            var result = validator.Validate(DateTime.Today);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithinDaysFromToday_With_Date_Too_Far_In_Past_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().WithinDaysFromToday(7);
            var tooOldDate = DateTime.Today.AddDays(-10);

            // Act
            var result = validator.Validate(tooOldDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be within 7 days from today.");
        }

        [Fact]
        public void WithinDaysFromToday_With_Date_Too_Far_In_Future_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().WithinDaysFromToday(7);
            var tooFarDate = DateTime.Today.AddDays(10);

            // Act
            var result = validator.Validate(tooFarDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must be within 7 days from today.");
        }

        [Fact]
        public void HasKind_With_Matching_Kind_Should_Pass()
        {
            // Arrange
            var validator = Validator.DateTime().HasKind(DateTimeKind.Utc);
            var utcDate = DateTime.SpecifyKind(_fixedDate, DateTimeKind.Utc);

            // Act
            var result = validator.Validate(utcDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void HasKind_With_Non_Matching_Kind_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().HasKind(DateTimeKind.Utc);
            var localDate = DateTime.SpecifyKind(_fixedDate, DateTimeKind.Local);

            // Act
            var result = validator.Validate(localDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must have DateTimeKind of Utc.");
        }

        [Fact]
        public void HasKind_With_Unspecified_Kind_Should_Fail()
        {
            // Arrange
            var validator = Validator.DateTime().HasKind(DateTimeKind.Local);
            var unspecifiedDate = DateTime.SpecifyKind(_fixedDate, DateTimeKind.Unspecified);

            // Act
            var result = validator.Validate(unspecifiedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Date must have DateTimeKind of Local.");
        }

        [Fact]
        public void Chained_DateTime_Validations_Should_All_Apply()
        {
            // Arrange
            var validator = Validator.DateTime()
                .After(_earlierDate)
                .Before(_laterDate)
                .NotDefault();

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Multiple_DateTime_Failures_Should_Return_All_Errors()
        {
            // Arrange
            var validator = Validator.DateTime()
                .After(_laterDate)
                .Before(_earlierDate)
                .IsToday();

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
        }

        [Fact]
        public void Custom_Error_Message_Should_Be_Used()
        {
            // Arrange
            var customMessage = "Custom date validation error";
            var validator = Validator.DateTime().After(_laterDate, customMessage);

            // Act
            var result = validator.Validate(_fixedDate);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Be(customMessage);
        }

        [Fact]
        public void TryValidate_With_Valid_Date_Should_Return_True()
        {
            // Arrange
            var validator = Validator.DateTime().After(_earlierDate);

            // Act
            var success = validator.TryValidate(_fixedDate, out var result);

            // Assert
            success.Should().BeTrue();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void TryValidate_With_Invalid_Date_Should_Return_False()
        {
            // Arrange
            var validator = Validator.DateTime().After(_laterDate);

            // Act
            var success = validator.TryValidate(_fixedDate, out var result);

            // Assert
            success.Should().BeFalse();
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateAndThrow_With_Invalid_Date_Should_Throw_Exception()
        {
            // Arrange
            var validator = Validator.DateTime().After(_laterDate);

            // Act & Assert
            var exception = Assert.Throws<ValidationLibrary.Core.ValidationException>(() => 
                validator.ValidateAndThrow(_fixedDate));
            
            exception.Message.Should().Contain("Date must be after");
        }
    }
}