using System;
using System.Collections.Generic;
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
    /// Validation modes for cabinet services.
    /// </summary>
    public enum ValidationMode
    {
        /// <summary>
        /// Default validation rules.
        /// </summary>
        Default,

        /// <summary>
        /// Custom validation rules.
        /// </summary>
        Custom,
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
            new Tuple<string, int>("use", AmountOfInputArgsForFullMode),
        };

        private static readonly Tuple<string, ValidationMode>[] RuleSet = new Tuple<string, ValidationMode>[]
        {
            new Tuple<string, ValidationMode>("default", ValidationMode.Default),
            new Tuple<string, ValidationMode>("custom", ValidationMode.Custom),
        };

        private static readonly Tuple<string, StorageMode>[] StorageSet = new Tuple<string, StorageMode>[]
        {
            new Tuple<string, StorageMode>("memory", StorageMode.Memory),
            new Tuple<string, StorageMode>("file", StorageMode.File),
        };

        private static readonly Tuple<string, Func<IFileCabinetService>>[] UsingModes = new Tuple<string, Func<IFileCabinetService>>[]
        {
            new Tuple<string, Func<IFileCabinetService>>("stopwatch", SwitchOnStopWatch),
            new Tuple<string, Func<IFileCabinetService>>("logger", SwitchOnLogger),
        };

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator chosenValidator;
        private static bool isRunning = true;

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

            var validationMode = ValidatorChooser(args);

            switch (validationMode)
            {
                case ValidationMode.Default: chosenValidator = new ValidatorBuilder().Default(); break;
                case ValidationMode.Custom: chosenValidator = new ValidatorBuilder().Custom(); break;
                default: chosenValidator = new ValidatorBuilder().Default(); break;
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            Console.WriteLine($"Using {validationMode} rules.");

            switch (StorageChooser(args))
            {
                case StorageMode.Memory: fileCabinetService = new FileCabinetMemoryService(chosenValidator); break;
                case StorageMode.File: fileCabinetService = new FileCabinetFilesystemService(new FileStream(CabinetRecordsFile, FileMode.Create), chosenValidator); break;
                default: fileCabinetService = new FileCabinetMemoryService(chosenValidator); break;
            }

            Console.WriteLine($"Strorage: {fileCabinetService}");

            ActivateUsingModes(args);

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
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);

            helpHandler.SetNext(createHandler)
                .SetNext(statHandler)
                .SetNext(listHandler)
                .SetNext(exportHandler)
                .SetNext(importHandler)
                .SetNext(purgeHandler)
                .SetNext(removeHandler)
                .SetNext(exitHandler)
                .SetNext(findHandler)
                .SetNext(editHandler)
                .SetNext(insertHandler)
                .SetNext(deleteHandler);

            return helpHandler;
        }

        private static ValidationMode ValidatorChooser(string[] args)
        {
            if (args.Length == 0)
            {
                return ValidationMode.Default;
            }

            var inputs = args.Length == AmountOfInputArgsForShortMode ? args : args.First().Split(DefaultStartupModeSeparator, 2);

            string inputRule;

            int indexOfMode = GetIndexOfMode(inputs, args);

            if (indexOfMode >= 0)
            {
                inputRule = inputs[^1];
            }
            else
            {
                return ValidationMode.Default;
            }

            int ruleIndex = Array.FindIndex(RuleSet, tuple => tuple.Item1.Equals(inputRule, StringComparison.InvariantCultureIgnoreCase));

            return ruleIndex >= 0 ? RuleSet[ruleIndex].Item2 : ValidationMode.Default;
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
                inputStorage = inputs[^1];
            }
            else
            {
                return StorageMode.Memory;
            }

            int storageIndex = Array.FindIndex(StorageSet, tuple => tuple.Item1.Equals(inputStorage, StringComparison.InvariantCultureIgnoreCase));

            return storageIndex >= 0 ? StorageSet[storageIndex].Item2 : StorageMode.Memory;
        }

        private static void ActivateUsingModes(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            var inputs = args.First().Split('-', 2);

            string inputUsingMode;

            int indexOfMode = GetIndexOfMode(inputs, args);

            if (indexOfMode >= 0)
            {
                inputUsingMode = inputs[^1];

                var switchOn = (from mode in UsingModes where mode.Item1.Equals(inputUsingMode, StringComparison.InvariantCultureIgnoreCase) select mode.Item2).FirstOrDefault();

                fileCabinetService = switchOn?.Invoke() ?? fileCabinetService;
            }
        }

        private static IFileCabinetService SwitchOnStopWatch()
        {
            return new ServiceMeter(fileCabinetService);
        }

        private static IFileCabinetService SwitchOnLogger()
        {
            return new ServiceLogger(SwitchOnStopWatch());
        }

        private static int GetIndexOfMode(string[] inputs, string[] args)
        {
            string inputMode = inputs.First();

            int indexOfMode = Array.FindIndex(StartupModes, mode => inputMode.Equals(mode.Item1, StringComparison.InvariantCultureIgnoreCase) && args.Length == mode.Item2);

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