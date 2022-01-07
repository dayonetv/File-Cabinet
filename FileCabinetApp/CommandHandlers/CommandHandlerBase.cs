using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for base implementation of methods for Command Handlers.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private const int DefaultMaxNameLength = 60;
        private const int DefaultMinSalary = 0;
        private const short DefaultMaxHeight = 220;

        private static readonly Predicate<char> DefaultGenderPredicate = new ((sex) => char.IsLetter(sex));
        private static readonly Predicate<DateTime> DefaultDateOfBirthPredicate = new ((date) => date < DateTime.Now);

        private ICommandHandler nextHandler;

        /// <inheritdoc/>
        public virtual void Handle(AppCommandRequest request)
        {
            if (request != null)
            {
                this.nextHandler?.Handle(request);
            }
        }

        /// <inheritdoc/>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return commandHandler;
        }

        /// <summary>
        /// Reads user string inputs.
        /// </summary>
        /// <typeparam name="T">returnong value type.</typeparam>
        /// <param name="converter">converter from string to any type.</param>
        /// <param name="validator">input validator.</param>
        /// <returns>Converted value from the string.</returns>
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

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

        /// <summary>
        /// Converts input to string.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, string> StringConverter(string input)
        {
            input = input?.Trim();
            bool result = !string.IsNullOrEmpty(input);
            return new Tuple<bool, string, string>(result, input, input);
        }

        /// <summary>
        /// Converts input to DateTime.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, DateTime> DateConverter(string input)
        {
            input = input?.Trim();
            bool result = DateTime.TryParseExact(input, "d", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime date);
            return new Tuple<bool, string, DateTime>(result, input, date);
        }

        /// <summary>
        /// Converts input to char.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, char> CharConverter(string input)
        {
            input = input?.Trim();
            bool result = char.TryParse(input, out char symbol);
            return new Tuple<bool, string, char>(result, input, symbol);
        }

        /// <summary>
        /// Converts input to decimal value.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, decimal> DecimalConverter(string input)
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
        protected static Tuple<bool, string, short> ShortConverter(string input)
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
        protected static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool result = firstName?.Length < DefaultMaxNameLength;
            return new Tuple<bool, string>(result, result ? "Valid" : "first name is too long");
        }

        /// <summary>
        /// Validates lastname of the record.
        /// </summary>
        /// <param name="lastName">lastname ptoperty to validate.</param>
        /// <returns>validation success, validation message.</returns>
        protected static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool result = lastName?.Length < DefaultMaxNameLength;
            return new Tuple<bool, string>(result, result ? "Valid" : "last name is too long");
        }

        /// <summary>
        /// Validates dateofbirth of the record.
        /// </summary>
        /// <param name="dateOfBirth">dateofbirth ptoperty to validate.</param>
        /// <returns>validation success, validation message.</returns>
        protected static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool result = DefaultDateOfBirthPredicate(dateOfBirth);
            return new Tuple<bool, string>(result, result ? "Valid" : "wrong date of birth");
        }

        /// <summary>
        /// Validates height of the record.
        /// </summary>
        /// <param name="height">height ptoperty to validate. </param>
        /// <returns>validation success, validation message.</returns>
        protected static Tuple<bool, string> HeightValidator(short height)
        {
            bool result = height < DefaultMaxHeight;
            return new Tuple<bool, string>(result, result ? "Valid" : "height is too big");
        }

        /// <summary>
        /// Validates salary of the record.
        /// </summary>
        /// <param name="salary">salary ptoperty to validate. </param>
        /// <returns>validation success, validation message.</returns>
        protected static Tuple<bool, string> SalaryValidator(decimal salary)
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
        protected static Tuple<bool, string> GenderValidator(char sex)
        {
            bool result = DefaultGenderPredicate(sex);
            return new Tuple<bool, string>(result, result ? "Valid" : "gender wrong format");
        }
    }
}
