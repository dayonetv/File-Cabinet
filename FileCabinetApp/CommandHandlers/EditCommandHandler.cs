using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for edit command and edit paramaters.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private const int DefaultMaxNameLength = 60;
        private const int CustomMaxNameLength = 100;
        private const int DefaultMinSalary = 0;
        private const int CustomMinSalary = 300;
        private const short DefaultMaxHeight = 220;
        private const short CustomMaxHeight = 250;

        private static readonly Predicate<char> DefaultGenderPredicate = new ((sex) => char.IsLetter(sex));
        private static readonly Predicate<char> CustomGenderPredicate = new ((sex) => !char.IsWhiteSpace(sex));
        private static readonly Predicate<DateTime> DefaultDateOfBirthPredicate = new ((date) => date < DateTime.Now);
        private static readonly Predicate<DateTime> CustomDateOfBirthPredicate = new ((date) => date <= DateTime.Now);

        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        public EditCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handles 'edit' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static CreateEditParameters EnterInfo()
        {
            CreateEditParameters parameters = new ();

            Console.Write("First Name: ");
            parameters.FirstName = ReadInput(StringConverter, FirstNameValidator);

            Console.Write("Last Name: ");
            parameters.LastName = ReadInput(StringConverter, LastNameValidator);

            Console.Write("Date Of Birth: ");
            parameters.DateOfBirth = ReadInput(DateConverter, DateOfBirthValidator);

            Console.Write("Height: ");
            parameters.Height = ReadInput(ShortConverter, HeightValidator);

            Console.Write("Salary: ");
            parameters.Salary = ReadInput(DecimalConverter, SalaryValidator);

            Console.Write("Sex: ");
            parameters.Sex = ReadInput(CharConverter, GenderValidator);

            return parameters;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, string> StringConverter(string input)
        {
            input = input.Trim();
            bool result = !string.IsNullOrEmpty(input);
            return new Tuple<bool, string, string>(result, input, input);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string input)
        {
            input = input.Trim();
            bool result = DateTime.TryParseExact(input, "d", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime date);
            return new Tuple<bool, string, DateTime>(result, input, date);
        }

        private static Tuple<bool, string, char> CharConverter(string input)
        {
            input = input.Trim();
            bool result = char.TryParse(input, out char symbol);
            return new Tuple<bool, string, char>(result, input, symbol);
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            input = input.Trim();
            bool result = decimal.TryParse(input, out decimal decValue);
            return new Tuple<bool, string, decimal>(result, input, decValue);
        }

        private static Tuple<bool, string, short> ShortConverter(string input)
        {
            input = input.Trim();
            bool result = short.TryParse(input, out short intValue);
            return new Tuple<bool, string, short>(result, input, intValue);
        }

        private static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool result = firstName.Length < (Program.ChosenValidator is DefaultValidator ? DefaultMaxNameLength : CustomMaxNameLength);
            return new Tuple<bool, string>(result, result ? "Valid" : "first name is too long");
        }

        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool result = lastName.Length < (Program.ChosenValidator is DefaultValidator ? DefaultMaxNameLength : CustomMaxNameLength);
            return new Tuple<bool, string>(result, result ? "Valid" : "last name is too long");
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool result = Program.ChosenValidator is DefaultValidator ? DefaultDateOfBirthPredicate(dateOfBirth) : CustomDateOfBirthPredicate(dateOfBirth);
            return new Tuple<bool, string>(result, result ? "Valid" : "wrong date of birth");
        }

        private static Tuple<bool, string> HeightValidator(short height)
        {
            bool result = height < (Program.ChosenValidator is DefaultValidator ? DefaultMaxHeight : CustomMaxHeight);
            return new Tuple<bool, string>(result, result ? "Valid" : "height is too big");
        }

        private static Tuple<bool, string> SalaryValidator(decimal salary)
        {
            int minSalary = Program.ChosenValidator is DefaultValidator ? DefaultMinSalary : CustomMinSalary;
            bool result = salary >= minSalary;
            return new Tuple<bool, string>(result, result ? "Valid" : $"salary can not be less than {minSalary}");
        }

        private static Tuple<bool, string> GenderValidator(char sex)
        {
            bool result = Program.ChosenValidator is DefaultValidator ? DefaultGenderPredicate(sex) : CustomGenderPredicate(sex);
            return new Tuple<bool, string>(result, result ? "Valid" : "gender wrong format");
        }

        private void Edit(string parameters)
        {
            bool isValid = true;
            do
            {
                try
                {
                    bool parseResult = int.TryParse(parameters, NumberStyles.Any, CultureInfo.InvariantCulture, out int id);

                    FileCabinetRecord recordToEdit = parseResult ? this.service.GetRecords()?.FirstOrDefault(rec => rec.Id == id) : null;

                    if (recordToEdit == null)
                    {
                        Console.WriteLine($"#{parameters} record is not found.");
                    }
                    else
                    {
                        CreateEditParameters updatedParams = EnterInfo();

                        this.service.EditRecord(id, updatedParams);

                        Console.WriteLine($"Record #{id} is updated.");

                        isValid = true;
                    }
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again.");
                }
            }
            while (!isValid);
        }
    }
}
