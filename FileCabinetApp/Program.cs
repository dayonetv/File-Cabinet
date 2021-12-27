using System;
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

        /// <summary>
        /// Gets or sets a value indicating whether the application is running now.
        /// </summary>
        /// <value></value>
        public static bool IsRunning { get; set; } = true;

        /// <summary>
        /// Gets or sets current service for storing records.
        /// </summary>
        /// <value></value>
        public static IFileCabinetService FileCabinetService { get; set; }

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

            var commandHandler = Program.CreateCommandHandlers();

            ChosenValidator = ValidatorChooser(args) ?? new DefaultValidator();

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            Console.WriteLine($"Using {ChosenValidator} rules.");

            switch (StorageChooser(args))
            {
                case StorageMode.Memory: FileCabinetService = new FileCabinetMemoryService(ChosenValidator); break;
                case StorageMode.File: FileCabinetService = new FileCabinetFilesystemService(new FileStream(CabinetRecordsFile, FileMode.Create), ChosenValidator); break;
                default: FileCabinetService = new FileCabinetMemoryService(ChosenValidator); break;
            }

            Console.WriteLine($"Strorage: {FileCabinetService}");

            Console.WriteLine(HintMessage);
            Console.WriteLine();

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
            while (IsRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler();
            var statHandler = new StatCommandHandler();
            var listHandler = new ListCommandHandler();
            var exportHandler = new ExportCommandHandler();
            var importHandler = new ImportCommandHandler();
            var purgeHandler = new PurgeCommandHandler();
            var removeHandler = new RemoveCommandHandler();
            var exitHandler = new ExitCommandHandler();
            var findHandler = new FindCommandHandler();
            var editHandler = new EditCommandHandler();

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
    }
}