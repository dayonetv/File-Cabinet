using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Konstantin Karasiov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DateFormat = "yyyy-MMM-dd";

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics", "The 'stat' command prints statistics" },
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "list", "prints current records", "The 'list' command prints current records." },
            new string[] { "edit", "edits record by id", "The 'edit' command edits record by id." },
            new string[] { "find", "finds record by some record field", "The 'find' command finds record by some record field." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
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
            bool isValid = true;

            do
            {
                try
                {
                    Console.Write("First Name: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Last Name: ");
                    string lastName = Console.ReadLine();

                    Console.Write("Date Of Birth: ");
                    DateTime dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "d", CultureInfo.InvariantCulture);

                    Console.Write("Height: ");
                    short height = short.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                    Console.Write("Salary: ");
                    decimal salary = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                    Console.Write("Sex: ");
                    char sex = Console.ReadKey().KeyChar;

                    Console.WriteLine($"\nRecord #{fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, height, salary, sex)} is created.");

                    isValid = true;
                }
                catch (FormatException ex)
                {
                    isValid = false;
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again. ");
                }
            }
            while (!isValid);
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
            try
            {
                int id;

                bool parseResult = int.TryParse(parameters, NumberStyles.Any, CultureInfo.InvariantCulture, out id);

                FileCabinetRecord recordToEdit = parseResult ? Array.Find(fileCabinetService.GetRecords(), rec => rec.Id == id) : null;

                if (recordToEdit == null)
                {
                    Console.WriteLine($"#{parameters} record is not found.");
                }
                else
                {
                    Console.Write("First Name: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Last Name: ");
                    string lastName = Console.ReadLine();

                    Console.Write("Date Of Birth: ");
                    DateTime dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "d", CultureInfo.InvariantCulture);

                    Console.Write("Height: ");
                    short height = short.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                    Console.Write("Salary: ");
                    decimal salary = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                    Console.Write("Sex: ");
                    char sex = Console.ReadKey().KeyChar;

                    fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, height, salary, sex);
                    Console.WriteLine($"\nRecord #{id} is updated.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"\n{ex.Message}. Try later.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}. Try later.");
            }
        }

        private static void Find(string parameters)
        {
            var inputParams = parameters.ToUpperInvariant().Trim().Split(' ', 2);

            string findBy = inputParams[0];

            FileCabinetRecord[] findedRecords = null;

            switch (findBy)
            {
                case "FIRSTNAME":
                    string firstNameToFind = inputParams[1].Trim('"');
                    findedRecords = fileCabinetService.FindByFirstName(firstNameToFind);
                    break;
                default:
                    findedRecords = Array.Empty<FileCabinetRecord>();
                    break;
            }

            if (findedRecords != Array.Empty<FileCabinetRecord>())
            {
                foreach (var record in findedRecords)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture)}");
                }
            }
            else
            {
                Console.WriteLine("No records finded");
            }
        }
    }
}