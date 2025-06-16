using ValidationLibrary;
using ValidationLibrary.Core;

namespace ValidationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ValidationLibrary Demo ===\n");

            // Run various validation examples
            RunStringValidationDemo();
            RunNumericValidationDemo();
            RunCollectionValidationDemo();
            RunObjectValidationDemo();
            RunConditionalValidationDemo();
            RunCustomValidationDemo();
            RunErrorHandlingDemo();

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RunStringValidationDemo()
        {
            Console.WriteLine("--- String Validation Demo ---");

            var nameValidator = Validator.String()
                .NotEmpty()
                .MinLength(2)
                .MaxLength(50)
                .Matches(@"^[a-zA-Z\s]+$", "Name must contain only letters and spaces");

            TestValidator("Valid Name", nameValidator, "John Doe");
            TestValidator("Short Name", nameValidator, "J");
            TestValidator("Name with Numbers", nameValidator, "John123");

            var emailValidator = Validator.String().Email();
            TestValidator("Valid Email", emailValidator, "user@example.com");
            TestValidator("Invalid Email", emailValidator, "not-an-email");

            Console.WriteLine();
        }

        static void RunNumericValidationDemo()
        {
            Console.WriteLine("--- Numeric Validation Demo ---");

            var ageValidator = Validator.Int()
                .GreaterThanOrEqual(0)
                .LessThanOrEqual(150);

            TestValidator("Valid Age", ageValidator, 25);
            TestValidator("Negative Age", ageValidator, -5);
            TestValidator("Too Old", ageValidator, 200);

            var scoreValidator = Validator.Double().Between(0.0, 100.0);
            TestValidator("Valid Score", scoreValidator, 85.5);
            TestValidator("Score Too High", scoreValidator, 101.0);

            Console.WriteLine();
        }

        static void RunCollectionValidationDemo()
        {
            Console.WriteLine("--- Collection Validation Demo ---");

            var tagsValidator = Validator.Array<string>()
                .NotNull()
                .MinCount(1)
                .MaxCount(5)
                .ForEach(Validator.String().NotEmpty().MaxLength(20));

            TestValidator("Valid Tags", tagsValidator, new[] { "developer", "csharp", "dotnet" });
            TestValidator("Empty Tags", tagsValidator, new string[0]);
            TestValidator("Tags with Empty String", tagsValidator, new[] { "valid", "", "also-valid" });

            Console.WriteLine();
        }

        static void RunObjectValidationDemo()
        {
            Console.WriteLine("--- Object Validation Demo ---");

            var userValidator = Validator.Object<User>()
                .NotNull()
                .RuleFor(u => u.Name, Validator.String().NotEmpty().MinLength(2))
                .RuleFor(u => u.Email, Validator.String().Email())
                .RuleFor(u => u.Age, Validator.Int().Between(0, 150))
                .RuleFor(u => u.Tags, Validator.Array<string>().MaxCount(5)
                    .ForEach(Validator.String().NotEmpty()))
                .Must(u => u.Age >= 18, "User must be at least 18 years old");

            var validUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Age = 25,
                Tags = new[] { "developer", "csharp" }
            };

            var invalidUser = new User
            {
                Name = "",
                Email = "invalid-email",
                Age = 16,
                Tags = new[] { "" }
            };

            TestValidator("Valid User", userValidator, validUser);
            TestValidator("Invalid User", userValidator, invalidUser);

            Console.WriteLine();
        }

        static void RunConditionalValidationDemo()
        {
            Console.WriteLine("--- Conditional Validation Demo ---");

            var conditionalValidator = Validator.Object<User>()
                .NotNull()
                .RuleFor(u => u.Name, Validator.String().NotEmpty())
                .When(u => u.Age >= 18, validator =>
                    validator.RuleFor(u => u.Email, Validator.String().Email()))
                .Unless(u => u.Age >= 21, validator =>
                    validator.Must(u => u.Tags?.Length > 0, "Young users must have tags"));

            var adultUser = new User { Name = "Adult", Email = "adult@example.com", Age = 25, Tags = new string[0] };
            var youngUser = new User { Name = "Young", Email = "", Age = 16, Tags = new[] { "young" } };

            TestValidator("Adult User", conditionalValidator, adultUser);
            TestValidator("Young User", conditionalValidator, youngUser);

            Console.WriteLine();
        }

        static void RunCustomValidationDemo()
        {
            Console.WriteLine("--- Custom Validation Demo ---");

            var evenLengthValidator = Validator.Custom<string>(
                value => value?.Length % 2 == 0,
                "String length must be even");

            TestValidator("Even Length String", evenLengthValidator, "test");
            TestValidator("Odd Length String", evenLengthValidator, "hello");

            var complexCustomValidator = Validator.Custom<string>(value =>
            {
                if (string.IsNullOrEmpty(value))
                    return ValidationResult.Failure("Value cannot be empty");

                if (!value.Any(char.IsUpper))
                    return ValidationResult.Failure("Must contain at least one uppercase letter");

                if (!value.Any(char.IsDigit))
                    return ValidationResult.Failure("Must contain at least one digit");

                return ValidationResult.Success();
            });

            TestValidator("Valid Complex", complexCustomValidator, "Hello123");
            TestValidator("No Uppercase", complexCustomValidator, "hello123");
            TestValidator("No Digit", complexCustomValidator, "Hello");

            Console.WriteLine();
        }

        static void RunErrorHandlingDemo()
        {
            Console.WriteLine("--- Error Handling Demo ---");

            var validator = Validator.String().NotEmpty().MinLength(5);

            // Exception handling
            try
            {
                var result = validator.ValidateAndThrow("Hi");
                Console.WriteLine("This shouldn't print");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }

            // TryValidate pattern
            if (validator.TryValidate("Hello", out var successResult))
            {
                Console.WriteLine("TryValidate: Success");
            }
            else
            {
                Console.WriteLine($"TryValidate: Failed - {successResult.ErrorMessage}");
            }

            if (!validator.TryValidate("Hi", out var failResult))
            {
                Console.WriteLine($"TryValidate: Failed - {failResult.ErrorMessage}");
            }

            Console.WriteLine();
        }

        static void TestValidator<T>(string testName, IValidator<T> validator, T value)
        {
            var result = validator.Validate(value);
            Console.WriteLine($"{testName}: {(result.IsValid ? "✓ Valid" : "✗ Invalid")}");
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    var path = string.IsNullOrEmpty(error.PropertyPath) ? "" : $"{error.PropertyPath}: ";
                    Console.WriteLine($"  - {path}{error.Message}");
                }
            }
        }
    }

    public class User
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int Age { get; set; }
        public string[] Tags { get; set; } = new string[0];
    }
}