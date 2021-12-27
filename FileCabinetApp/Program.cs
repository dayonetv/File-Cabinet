using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Storage modes for cabinet service.
    /// </summary>
    public enum StorageMode
    {
        /// <summary>
        /// Mode for using FileCabinetMemoryService
        /// </summary>
        Memory,

        /// <summary>
        /// Mode for using FileCabinetFilesystemService
        /// </summary>
        File,
    }

    /// <summary>
    /// Represents the main interface for user to use corresponding commands.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Konstantin Karasiov";
        private const string CabinetRecordsFile = "cabinet-records.db";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DateFormat = "d";

        private const int AmountOfInputArgsForShortMode = 2;
        private const int AmountOfInputArgsForFullMode = 1;
        private const char DefaultStartupModeSeparator = '=';

        private static readonly Tuple<string, int>[] StartupModes = new Tuple<string, int>[]
        {
            new Tuple<string, int>("--validation-rules", AmountOfInputArgsForFullMode),
            new Tuple<string, int>("-v", AmountOfInputArgsForShortMode),
            new Tuple<string, int>("--storage", AmountOfInputArgsForFullMode),
            new Tuple<string, int>("-s", AmountOfInputArgsForShortMode),
        };

        private static readonly Tuple<string, IRecordValidator>[] RuleSet = new Tuple<string, IRecordValidator>[]
        {
            new Tuple<string, IRecordValidator>("default", new DefaultValidator()),
            new Tuple<string, IRecordValidator>("custom", new CustomValidator()),
        };

        private static readonly Tuple<string, StorageMode>[] StorageSet = new Tuple<string, StorageMode>[]
        {
            new Tuple<string, StorageMode>("memory", StorageMode.Memory),
            new Tuple<string, StorageMode>("file", StorageMode.File),
        };

        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        /// <summary>
        /// Gets or sets current valitor for records.
        /// </summary>
        /// <value></value>
        public static IRecordValidator ChosenValidator { get; set; }

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

            ChosenValidator = ValidatorChooser(args) ?? new DefaultValidator();

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            Console.WriteLine($"Using {ChosenValidator} rules.");

            switch (StorageChooser(args))
            {
                case StorageMode.Memory: fileCabinetService = new FileCabinetMemoryService(ChosenValidator); break;
                case StorageMode.File: fileCabinetService = new FileCabinetFilesystemService(new FileStream(CabinetRecordsFile, FileMode.Create), ChosenValidator); break;
                default: fileCabinetService = new FileCabinetMemoryService(ChosenValidator); break;
            }

            Console.WriteLine($"Strorage: {fileCabinetService}");

            Console.WriteLine(HintMessage);
            Console.WriteLine();

            var commandHandler = Program.CreateCommandHandlers();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);

                const int commandIndex = 0;
                const int parametersIndex = 1;

                var command = inputs[commandIndex];
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                commandHandler.Handle(new AppCommandRequest()
                {
                    Command = command,
                    Parameters = parameters,
                });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, Program.DefaultRecordPrint);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler((state) => isRunning = state);
            var findHandler = new FindCommandHandler(fileCabinetService, Program.DefaultRecordPrint);
            var editHandler = new EditCommandHandler(fileCabinetService);

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(statHandler);
            statHandler.SetNext(listHandler);
            listHandler.SetNext(exportHandler);
            exportHandler.SetNext(importHandler);
            importHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(removeHandler);
            removeHandler.SetNext(exitHandler);
            exitHandler.SetNext(findHandler);
            findHandler.SetNext(editHandler);
            editHandler.SetNext(null);

            return helpHandler;
        }

        private static IRecordValidator ValidatorChooser(string[] args)
        {
            if (args.Length == 0)
            {
                return new DefaultValidator();
            }

            var inputs = args.Length == AmountOfInputArgsForShortMode ? args : args.First().Split(DefaultStartupModeSeparator, 2);

            string inputRule;

            int indexOfMode = GetIndexOfMode(inputs, args);

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

        private static StorageMode StorageChooser(string[] args)
        {
            if (args.Length == 0)
            {
                return StorageMode.Memory;
            }

            var inputs = args.Length == AmountOfInputArgsForShortMode ? args : args.First().Split(DefaultStartupModeSeparator, 2);

            string inputStorage;

            int indexOfMode = GetIndexOfMode(inputs, args);

            if (indexOfMode >= 0)
            {
                inputStorage = inputs[1];
            }
            else
            {
                return StorageMode.Memory;
            }

            int storageIndex = Array.FindIndex(StorageSet, tuple => tuple.Item1.Equals(inputStorage, StringComparison.InvariantCultureIgnoreCase));

            return storageIndex >= 0 ? StorageSet[storageIndex].Item2 : StorageMode.Memory;
        }

        private static int GetIndexOfMode(string[] inputs, string[] args)
        {
            string inputMode = inputs.First();

            int indexOfMode = Array.FindIndex(StartupModes, mode => inputMode.Equals(mode.Item1, StringComparison.InvariantCultureIgnoreCase) && args.Length == mode.Item2);
            if (indexOfMode < 0)
            {
                Console.WriteLine($"Unknown strart up arguments: {string.Join(' ', args)}");
            }

            return indexOfMode;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture)}, {record.Height}, {record.Salary}, {record.Sex}");
            }
        }
    }
}