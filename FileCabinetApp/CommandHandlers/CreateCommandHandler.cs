using System;
using System.Globalization;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "create";

        private const int DefaultMaxNameLength = 60;
        private const int DefaultMinSalary = 0;
        private const short DefaultMaxHeight = 220;

        private static readonly Predicate<char> DefaultGenderPredicate = new ((sex) => char.IsLetter(sex));
        private static readonly Predicate<DateTime> DefaultDateOfBirthPredicate = new ((date) => date < DateTime.Now);

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'create' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create();
            }
            else
            {
                base.Handle(request);
            }
        }

        /// <summary>
        /// Converts input to decimal value.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        private static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            input = input?.Trim();
            bool result = decimal.TryParse(input, out decimal decValue);
            return new Tuple<bool, string, decimal>(result, input, decValue);
        }

        /// <summary>
        /// Converts input to short value.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        private static Tuple<bool, string, short> ShortConverter(string input)
        {
            input = input?.Trim();
            bool result = short.TryParse(input, out short intValue);
            return new Tuple<bool, string, short>(result, input, intValue);
        }

        /// <summary>
        /// Validates firstname of the record.
        /// </summary>
        /// <param name="firstName">firstname ptoperty to validate. </param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool result = firstName?.Length < DefaultMaxNameLength;
            return new Tuple<bool, string>(result, result ? "Valid" : "first name is too long");
        }

        /// <summary>
        /// Validates lastname of the record.
        /// </summary>
        /// <param name="lastName">lastname ptoperty to validate.</param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool result = lastName?.Length < DefaultMaxNameLength;
            return new Tuple<bool, string>(result, result ? "Valid" : "last name is too long");
        }

        /// <summary>
        /// Validates dateofbirth of the record.
        /// </summary>
        /// <param name="dateOfBirth">dateofbirth ptoperty to validate.</param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool result = DefaultDateOfBirthPredicate(dateOfBirth);
            return new Tuple<bool, string>(result, result ? "Valid" : "wrong date of birth");
        }

        /// <summary>
        /// Validates height of the record.
        /// </summary>
        /// <param name="height">height ptoperty to validate. </param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> HeightValidator(short height)
        {
            bool result = height < DefaultMaxHeight;
            return new Tuple<bool, string>(result, result ? "Valid" : "height is too big");
        }

        /// <summary>
        /// Validates salary of the record.
        /// </summary>
        /// <param name="salary">salary ptoperty to validate. </param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> SalaryValidator(decimal salary)
        {
            int minSalary = DefaultMinSalary;
            bool result = salary >= minSalary;
            return new Tuple<bool, string>(result, result ? "Valid" : $"salary can not be less than {minSalary}");
        }

        /// <summary>
        /// Validates gender of the record.
        /// </summary>
        /// <param name="sex">gender ptoperty to validate.</param>
        /// <returns>validation success, validation message.</returns>
        private static Tuple<bool, string> GenderValidator(char sex)
        {
            bool result = DefaultGenderPredicate(sex);
            return new Tuple<bool, string>(result, result ? "Valid" : "gender wrong format");
        }

        /// <summary>
        /// Converts input to DateTime.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        private static Tuple<bool, string, DateTime> DateConverter(string input)
        {
            input = input?.Trim();
            bool result = DateTime.TryParseExact(input, "d", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime date);
            return new Tuple<bool, string, DateTime>(result, input, date);
        }

        /// <summary>
        /// Reads console inputs for creating records parameters.
        /// </summary>
        /// <returns>Parameters entered by user.</returns>
        private static RecordParameters EnterInfo()
        {
            RecordParameters parameters = new ();

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

        private void Create()
        {
            bool isValid;
            do
            {
                try
                {
                    RecordParameters creationParams = EnterInfo();

                    Console.WriteLine($"Record #{this.Service.CreateRecord(creationParams)} is created.");

                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again. ");
                }
            }
            while (!isValid);
        }
    }
}
