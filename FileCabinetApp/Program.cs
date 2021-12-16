﻿using System;
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

        private static readonly Tuple<string, Func<string, FileCabinetRecord[]>>[] FindByFunctions = new Tuple<string, Func<string, FileCabinetRecord[]>>[]
        {
            new Tuple<string, Func<string, FileCabinetRecord[]>>("firstname", FindByFirstName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("lastname", FindByLastName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("dateofbirth", FindByDateOfBirth),
        };

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        /// <summary>
        /// The main console application entry point.
        /// </summary>
        /// <param name="args">Applicattion startup parameters. </param>
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
                catch (FormatException ex)
                {
                    isValid = false;
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}.\nPlease try again. ");
                }
            }
            while (!isValid);
        }

        private static CreateEditParameters EnterInfo()
        {
            CreateEditParameters parameters = new ();

            Console.Write("First Name: ");
            parameters.FirstName = Console.ReadLine().Trim();

            Console.Write("Last Name: ");
            parameters.LastName = Console.ReadLine().Trim();

            Console.Write("Date Of Birth: ");
            parameters.DateOfBirth = DateTime.ParseExact(Console.ReadLine().Trim(), "d", CultureInfo.InvariantCulture);

            Console.Write("Height: ");
            parameters.Height = short.Parse(Console.ReadLine().Trim(), CultureInfo.InvariantCulture);

            Console.Write("Salary: ");
            parameters.Salary = decimal.Parse(Console.ReadLine().Trim(), CultureInfo.InvariantCulture);

            Console.Write("Sex: ");
            parameters.Sex = Console.ReadKey().KeyChar;

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

                    FileCabinetRecord recordToEdit = parseResult ? Array.Find(fileCabinetService.GetRecords(), rec => rec.Id == id) : null;

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
                catch (FormatException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again.");
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}.\nPlease try again.");
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

            FileCabinetRecord[] findedRecords = findByFunc.Invoke(toFind);

            if (findedRecords != Array.Empty<FileCabinetRecord>())
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

        private static FileCabinetRecord[] FindByDateOfBirth(string dateToFind)
        {
            bool parseResult = DateTime.TryParseExact(dateToFind.Trim('"'), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBithToFind);
            return parseResult ? fileCabinetService.FindByDateOfBith(dateOfBithToFind) : Array.Empty<FileCabinetRecord>();
        }

        private static FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return fileCabinetService.FindByFirstName(firstName.Trim('"'));
        }

        private static FileCabinetRecord[] FindByLastName(string lastName)
        {
            return fileCabinetService.FindByLastName(lastName.Trim('"'));
        }
    }
}