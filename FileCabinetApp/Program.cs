using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents the main interface for user to use corresponding commands.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Konstantin Karasiov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DateFormat = "yyyy-MMM-dd";

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const int AmountOfFindByParams = 2;
        private const int AmountOfInputArgsForShortMode = 2;
        private const int AmountOfInputArgsForFullMode = 1;
        private const char FullStartupModeSeparator = '=';

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

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics", "The 'stat' command prints statistics" },
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "list", "prints current records", "The 'list' command prints current records." },
            new string[] { "edit", "edits record by id", "The 'edit' command edits record by id." },
            new string[] { "find", "finds record by some record field", "The 'find' command finds record by some record field." },
        };

        private static readonly Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] FindByFunctions = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
        {
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", FindByFirstName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", FindByLastName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", FindByDateOfBirth),
        };

        private static readonly Tuple<string, int>[] StartupModes = new Tuple<string, int>[]
        {
            new Tuple<string, int>("--validation-rules", AmountOfInputArgsForFullMode),
            new Tuple<string, int>("-v", AmountOfInputArgsForShortMode),
        };

        private static readonly Tuple<string, IRecordValidator>[] RuleSet = new Tuple<string, IRecordValidator>[]
        {
            new Tuple<string, IRecordValidator>("default", new DefaultValidator()),
            new Tuple<string, IRecordValidator>("custom", new CustomValidator()),
        };

        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator chosenValidator;

        /// <summary>
        /// The main console-application entry point.
        /// </summary>
        /// <param name="args">Applicattion startup parameters. </param>
        public static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            chosenValidator = ValidateInputArgs(args);

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            switch (chosenValidator)
            {
                case DefaultValidator: Console.WriteLine("Using default validation rules."); break;
                case CustomValidator: Console.WriteLine("Using custom validation rules."); break;
                default: Console.WriteLine($"Unknown startup arguments: {string.Join(' ', args)}"); return;
            }

            fileCabinetService = new FileCabinetService(chosenValidator);

            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static IRecordValidator ValidateInputArgs(string[] args)
        {
            if (args.Length == 0)
            {
                return new DefaultValidator();
            }

            var inputs = args.Length == AmountOfInputArgsForShortMode ? args : args.First().Split(FullStartupModeSeparator, 2);

            string inputRule;
            string inputMode;

            inputMode = inputs.First();

            int indexOfMode = Array.FindIndex(StartupModes, mode => inputMode.Equals(mode.Item1, StringComparison.InvariantCultureIgnoreCase) && args.Length == mode.Item2);
            if (indexOfMode >= 0)
            {
                inputRule = inputs[1];
            }
            else
            {
                return null;
            }

            int ruleIndex = Array.FindIndex(RuleSet, tuple => tuple.Item1.Equals(inputRule, StringComparison.InvariantCultureIgnoreCase));

            return ruleIndex >= 0 ? RuleSet[ruleIndex].Item2 : null;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            bool isValid;
            do
            {
                try
                {
                    CreateEditParameters creationParams = EnterInfo();

                    Console.WriteLine($"\nRecord #{fileCabinetService.CreateRecord(creationParams)} is created.");

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

        private static void List(string parameters)
        {
            foreach (var record in fileCabinetService.GetRecords())
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture)}, {record.Height}, {record.Salary}, {record.Sex}");
            }
        }

        private static void Edit(string parameters)
        {
            bool isValid = true;
            do
            {
                try
                {
                    bool parseResult = int.TryParse(parameters, NumberStyles.Any, CultureInfo.InvariantCulture, out int id);

                    FileCabinetRecord recordToEdit = parseResult ? fileCabinetService.GetRecords()?.FirstOrDefault(rec => rec.Id == id) : null;

                    if (recordToEdit == null)
                    {
                        Console.WriteLine($"#{parameters} record is not found.");
                    }
                    else
                    {
                        CreateEditParameters updatedParams = EnterInfo();

                        fileCabinetService.EditRecord(id, updatedParams);

                        Console.WriteLine($"\nRecord #{id} is updated.");

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

        private static void Find(string parameters)
        {
            var inputParams = parameters.Trim().Split(' ', AmountOfFindByParams);

            if (inputParams.Length != AmountOfFindByParams)
            {
                Console.WriteLine($"'find' command requires at least {AmountOfFindByParams} parameters. ");
                return;
            }

            string findBy = inputParams[0].Trim();
            string toFind = inputParams[^1].Trim();

            var findByFunc = (from func in FindByFunctions where func.Item1.Equals(findBy, StringComparison.InvariantCultureIgnoreCase) select func.Item2).FirstOrDefault();

            if (findByFunc == null)
            {
                Console.WriteLine($"Unknown '{findBy}' property for 'find' command. ");
                return;
            }

            ReadOnlyCollection<FileCabinetRecord> findedRecords = findByFunc.Invoke(toFind);

            if (findedRecords != null)
            {
                foreach (var record in findedRecords)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture)}");
                }
            }
            else
            {
                Console.WriteLine($"There is no records with {findBy}: '{toFind}'. ");
            }
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateToFind)
        {
            bool parseResult = DateTime.TryParseExact(dateToFind.Trim('"'), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBithToFind);
            return parseResult ? fileCabinetService.FindByDateOfBith(dateOfBithToFind) : null;
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return fileCabinetService.FindByFirstName(firstName.Trim('"'));
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return fileCabinetService.FindByLastName(lastName.Trim('"'));
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
            bool result = firstName.Length < (chosenValidator is DefaultValidator ? DefaultMaxNameLength : CustomMaxNameLength);
            return new Tuple<bool, string>(result, result ? "Valid" : "first name is too long");
        }

        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool result = lastName.Length < (chosenValidator is DefaultValidator ? DefaultMaxNameLength : CustomMaxNameLength);
            return new Tuple<bool, string>(result, result ? "Valid" : "last name is too long");
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool result = chosenValidator is DefaultValidator ? DefaultDateOfBirthPredicate(dateOfBirth) : CustomDateOfBirthPredicate(dateOfBirth);
            return new Tuple<bool, string>(result, result ? "Valid" : "wrong date of birth");
        }

        private static Tuple<bool, string> HeightValidator(short height)
        {
            bool result = height < (chosenValidator is DefaultValidator ? DefaultMaxHeight : CustomMaxHeight);
            return new Tuple<bool, string>(result, result ? "Valid" : "height is too big");
        }

        private static Tuple<bool, string> SalaryValidator(decimal salary)
        {
            int minSalary = chosenValidator is DefaultValidator ? DefaultMinSalary : CustomMinSalary;
            bool result = salary >= minSalary;
            return new Tuple<bool, string>(result, result ? "Valid" : $"salary can not be less than {minSalary}");
        }

        private static Tuple<bool, string> GenderValidator(char sex)
        {
            bool result = chosenValidator is DefaultValidator ? DefaultGenderPredicate(sex) : CustomGenderPredicate(sex);
            return new Tuple<bool, string>(result, result ? "Valid" : "gender wrong format");
        }
    }
}