using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// fdhg.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Sartup message.
        /// </summary>
        public const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DateFormat = "yyyy-MMM-dd";

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const int AmountOfFindByParams = 2;
        private const int AmountOfExportParams = 2;
        private const int AmountOFImportParams = 2;

        private const int DefaultMaxNameLength = 60;
        private const int CustomMaxNameLength = 100;
        private const int DefaultMinSalary = 0;
        private const int CustomMinSalary = 300;
        private const short DefaultMaxHeight = 220;
        private const short CustomMaxHeight = 250;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
{
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
};

        private static readonly Predicate<char> DefaultGenderPredicate = new ((sex) => char.IsLetter(sex));
        private static readonly Predicate<char> CustomGenderPredicate = new ((sex) => !char.IsWhiteSpace(sex));
        private static readonly Predicate<DateTime> DefaultDateOfBirthPredicate = new ((date) => date < DateTime.Now);
        private static readonly Predicate<DateTime> CustomDateOfBirthPredicate = new ((date) => date <= DateTime.Now);

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics", "The 'stat' command prints statistics" },
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "list", "prints current records", "The 'list' command prints current records." },
            new string[] { "edit", "edits record by id", "The 'edit' command edits record by id." },
            new string[] { "find", "finds record by some record field", "The 'find' command finds record by some record field." },
            new string[] { "export", "exports all records to the file", "The 'export' command exports all records to the file." },
            new string[] { "import", "imports records from the file", "The 'import' command imports records from the file." },
            new string[] { "remove", "removes record by its id", "The 'remove' command removes record by its id." },
            new string[] { "purge", "defragmentates records file for Filesystem Service", "The 'purge' defragmentates records file for Filesystem Service." },
        };

        private static readonly Tuple<char, bool>[] Choices = new Tuple<char, bool>[]
        {
            new Tuple<char, bool>('Y', true),
            new Tuple<char, bool>('N', false),
        };

        private static readonly Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] FindByFunctions = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
        {
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", FindByFirstName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", FindByLastName),
            new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", FindByDateOfBirth),
        };

        private static readonly Tuple<string, Func<FileInfo, bool, string>>[] SavingModes = new Tuple<string, Func<FileInfo, bool, string>>[]
        {
            new Tuple<string, Func<FileInfo, bool, string>>("csv", WriteToCsv),
            new Tuple<string, Func<FileInfo, bool, string>>("xml", WriteToXml),
        };

        private static readonly Tuple<string, Func<FileInfo, string>>[] ImportModes = new Tuple<string, Func<FileInfo, string>>[]
        {
            new Tuple<string, Func<FileInfo, string>>("csv", ImportFromCsv),
            new Tuple<string, Func<FileInfo, string>>("xml", ImportFromXml),
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                Console.WriteLine(CommandHandler.HintMessage);
                return;
            }

            if (string.IsNullOrEmpty(request.Command))
            {
                Console.WriteLine(CommandHandler.HintMessage);
                return;
            }

            var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(request.Command, StringComparison.InvariantCultureIgnoreCase));

            if (index >= 0)
            {
                Commands[index].Item2(request.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(request.Command);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }

        private static void Stat(string parameters)
        {
            var totalRecordsCount = Program.FileCabinetService.GetStat();
            var deletedRecordsCount = totalRecordsCount - Program.FileCabinetService.GetRecords().Count;

            Console.WriteLine($"Total: {totalRecordsCount} record(s).\nDeleted: {deletedRecordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            bool isValid;
            do
            {
                try
                {
                    CreateEditParameters creationParams = EnterInfo();

                    Console.WriteLine($"Record #{Program.FileCabinetService.CreateRecord(creationParams)} is created.");

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
            foreach (var record in Program.FileCabinetService.GetRecords())
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

                    FileCabinetRecord recordToEdit = parseResult ? Program.FileCabinetService.GetRecords()?.FirstOrDefault(rec => rec.Id == id) : null;

                    if (recordToEdit == null)
                    {
                        Console.WriteLine($"#{parameters} record is not found.");
                    }
                    else
                    {
                        CreateEditParameters updatedParams = EnterInfo();

                        Program.FileCabinetService.EditRecord(id, updatedParams);

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

        private static void Export(string parameters)
        {
            var inputParams = parameters.Trim().Split(' ', AmountOfExportParams);

            if (inputParams.Length != AmountOfExportParams)
            {
                Console.WriteLine($"'export' command requires at least {AmountOfExportParams} parameters. ");
                return;
            }

            string fileExtention = inputParams[0].Trim();
            string fileName = inputParams[^1].Trim();

            FileInfo exportFile = new FileInfo(fileName);

            bool toRewrite = true;

            if (exportFile.Exists)
            {
                Console.WriteLine($"File is exist - rewrite {exportFile.FullName}? [Y/n] ");
                char inputChoice = ReadInput(CharConverter, YesNoChoiceValidator);

                int choiseIndex = Array.FindIndex(Choices, choice => choice.Item1.ToString().Equals(inputChoice.ToString(), StringComparison.InvariantCultureIgnoreCase));
                toRewrite = Choices[choiseIndex].Item2;
            }

            int saveModeIndex = Array.FindIndex(SavingModes, (mode) => mode.Item1.Equals(fileExtention, StringComparison.InvariantCultureIgnoreCase));

            if (saveModeIndex >= 0)
            {
                try
                {
                    Console.WriteLine(SavingModes[saveModeIndex].Item2.Invoke(exportFile, toRewrite));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Unknown: {fileExtention} parameter for 'export' command");
            }
        }

        private static string WriteToCsv(FileInfo fileToWriteTo, bool rewrite)
        {
            if (rewrite)
            {
                StreamWriter writer = fileToWriteTo.CreateText();

                var snapShot = Program.FileCabinetService.MakeSnapShot();
                snapShot.SaveToScv(writer);

                writer.Close();

                return $"All records are exported to file {fileToWriteTo.FullName}.";
            }

            return $"Saving canceled";
        }

        private static string WriteToXml(FileInfo fileToWriteTo, bool rewrite)
        {
            if (rewrite)
            {
                StreamWriter writer = fileToWriteTo.CreateText();

                var snapShot = Program.FileCabinetService.MakeSnapShot();
                snapShot.SaveToXml(writer);

                writer.Close();

                return $"All records are exported to file {fileToWriteTo.FullName}.";
            }

            return $"Saving canceled";
        }

        private static void Import(string parameters)
        {
            var inputParams = parameters.Trim().Split(' ', AmountOFImportParams);

            if (inputParams.Length != AmountOFImportParams)
            {
                Console.WriteLine($"'import' command requires at least {AmountOFImportParams} parameters. ");
                return;
            }

            string importMode = inputParams[0].Trim();
            string fileName = inputParams[^1].Trim();

            FileInfo importFile = new FileInfo(fileName);

            if (importFile.Exists)
            {
                int importModeIndex = Array.FindIndex(ImportModes, (tuple) => tuple.Item1.Equals(importMode, StringComparison.InvariantCultureIgnoreCase));
                if (importModeIndex >= 0)
                {
                    try
                    {
                        string message = ImportModes[importModeIndex].Item2?.Invoke(importFile);
                        Console.WriteLine(message);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown import mode: {importMode}.");
                }
            }
            else
            {
                Console.WriteLine($"Import error: file {importFile.FullName} is not exist.");
            }
        }

        private static string ImportFromCsv(FileInfo fileToImportFrom)
        {
            StreamReader csvReader = fileToImportFrom.OpenText();

            try
            {
                var snapshot = new FileCabinetServiceSnapshot();
                snapshot.LoadFromScv(csvReader);
                string restoringMessage = Program.FileCabinetService.Restore(snapshot);

                return $"{restoringMessage} from {fileToImportFrom.FullName}.";
            }
            catch (IOException ex)
            {
                return $"Import error: {ex.Message}";
            }
            catch (FormatException ex)
            {
                return $"Import error: {ex.Message}";
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return $"Import error: {ex.Message}";
            }
            finally
            {
                csvReader.Close();
            }
        }

        private static string ImportFromXml(FileInfo fileToImportFrom)
        {
            FileStream xmlReader = new FileStream(fileToImportFrom.FullName, FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                var snapshot = new FileCabinetServiceSnapshot();
                snapshot.LoadFromXml(xmlReader);
                string restoringMessage = Program.FileCabinetService.Restore(snapshot);

                return $"{restoringMessage} from {fileToImportFrom.FullName}.";
            }
            catch (IOException ex)
            {
                return $"Import error: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"Import error: {ex.Message}";
            }
            finally
            {
                xmlReader.Close();
            }
        }

        private static void Remove(string parameters)
        {
            bool parseResult = int.TryParse(parameters.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int id);

            if (parseResult)
            {
                bool removingResult = Program.FileCabinetService.Remove(id);

                Console.WriteLine(removingResult ? $"Record #{id} is removed." : $"Record #{id} doesn't exists.");
                return;
            }

            Console.WriteLine($"Invalid format for id: {parameters}");
        }

        private static void Purge(string parameters)
        {
            if (Program.FileCabinetService is FileCabinetMemoryService)
            {
                Console.WriteLine($"{Program.FileCabinetService} has nothing to purge. ");
                return;
            }

            int allrecordsAmount = Program.FileCabinetService.GetStat();
            int purgedAmount = Program.FileCabinetService.Purge();

            Console.WriteLine($"Data file processing is completed: {purgedAmount} of {allrecordsAmount} records were purged.");
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateToFind)
        {
            bool parseResult = DateTime.TryParseExact(dateToFind.Trim('"'), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBithToFind);
            return parseResult ? Program.FileCabinetService.FindByDateOfBith(dateOfBithToFind) : null;
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return Program.FileCabinetService.FindByFirstName(firstName.Trim('"'));
        }

        private static ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return Program.FileCabinetService.FindByLastName(lastName.Trim('"'));
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

        private static Tuple<bool, string> YesNoChoiceValidator(char inputChoice)
        {
            bool result = Array.FindIndex(Choices, choice => choice.Item1.ToString().Equals(inputChoice.ToString(), StringComparison.InvariantCultureIgnoreCase)) >= 0;
            return new Tuple<bool, string>(result, result ? "Valid" : "Choice can only be 'Y' or 'N'");
        }
    }
}
