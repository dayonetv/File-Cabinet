using System;
using System.IO;
using System.Linq;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.RecordValidators;
using FileCabinetApp.Services;

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
    /// Validation modes for cabinet service.
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

        private const char DefaultStartupModeSeparator = '=';

        private const string ValidationFullCommand = "--validation-rules";
        private const string ValidationShortCommand = "-v";

        private const string StorageFullCommand = "--storage";
        private const string StorageShortCommand = "-s";

        private const string UseCommand = "use";

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
        /// <param name="args">Application startup parameters. </param>
        public static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            ChooseValidatior(args);

            ChooseStorage(args);

            Console.WriteLine($"Strorage: {fileCabinetService}");

            ActivateUsingModes(args);

            Console.WriteLine(HintMessage);
            Console.WriteLine();

            var commandHandler = Program.CreateCommandHandlers();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2, StringSplitOptions.TrimEntries);

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
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler((state) => isRunning = state);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService);

            helpHandler.SetNext(createHandler)
                .SetNext(statHandler)
                .SetNext(exportHandler)
                .SetNext(importHandler)
                .SetNext(purgeHandler)
                .SetNext(exitHandler)
                .SetNext(selectHandler)
                .SetNext(updateHandler)
                .SetNext(insertHandler)
                .SetNext(deleteHandler);

            return helpHandler;
        }

        private static void ActivateUsingModes(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            int useCommandIndex = Array.FindIndex(args, (arg) => arg.StartsWith(UseCommand, StringComparison.InvariantCultureIgnoreCase));

            if (useCommandIndex < 0)
            {
                return;
            }

            var inputs = args[useCommandIndex].Split('-', 2);

            string inputUsingMode = inputs.Last();

            var switchOn = (from mode in UsingModes where mode.Item1.Equals(inputUsingMode, StringComparison.InvariantCultureIgnoreCase) select mode.Item2).FirstOrDefault();

            fileCabinetService = switchOn?.Invoke() ?? fileCabinetService;
        }

        private static IFileCabinetService SwitchOnStopWatch()
        {
            Console.WriteLine("Using stopwatch.");
            return new ServiceMeter(fileCabinetService);
        }

        private static IFileCabinetService SwitchOnLogger()
        {
            Console.WriteLine("Using logger.");
            return new ServiceLogger(SwitchOnStopWatch());
        }

        private static void ChooseValidatior(string[] args)
        {
            int validationCommandIndex = Array.FindIndex(args, (arg) => arg.StartsWith(ValidationFullCommand, StringComparison.InvariantCultureIgnoreCase));

            if (validationCommandIndex >= 0)
            {
                ProcessValidationFullCommand(args, validationCommandIndex);
                return;
            }

            validationCommandIndex = Array.FindIndex(args, (arg) => arg.Equals(ValidationShortCommand, StringComparison.InvariantCultureIgnoreCase));

            if (validationCommandIndex >= 0)
            {
                ProcessValidationShortCommand(args, validationCommandIndex);
                return;
            }

            chosenValidator = new ValidatorBuilder().Default();

            Console.WriteLine($"Using {ValidationMode.Default} rules.");
        }

        private static void ProcessValidationFullCommand(string[] args, int validationCommandIndex)
        {
            var inputs = args[validationCommandIndex].Split(DefaultStartupModeSeparator, StringSplitOptions.TrimEntries);

            string inputRule = inputs.Last();

            SetValidator(inputRule);
        }

        private static void ProcessValidationShortCommand(string[] args, int validationCommandIndex)
        {
            if (validationCommandIndex == args.Length - 1)
            {
                chosenValidator = new ValidatorBuilder().Default();
                Console.WriteLine($"Using {ValidationMode.Default} rules.");

                return;
            }

            string inputRule = args[validationCommandIndex + 1].Trim();

            SetValidator(inputRule);
        }

        private static void SetValidator(string inputRule)
        {
            int indexOfMode = Array.FindIndex(RuleSet, (rule) => rule.Item1.Equals(inputRule, StringComparison.InvariantCultureIgnoreCase));

            if (indexOfMode >= 0)
            {
                switch (RuleSet[indexOfMode].Item2)
                {
                    case ValidationMode.Default: chosenValidator = new ValidatorBuilder().Default(); break;
                    case ValidationMode.Custom: chosenValidator = new ValidatorBuilder().Custom(); break;
                }

                Console.WriteLine($"Using {RuleSet[indexOfMode].Item2} rules.");
            }
            else
            {
                Console.WriteLine($"Unknown validation rule: '{inputRule}'.");

                chosenValidator = new ValidatorBuilder().Default();

                Console.WriteLine($"Using {ValidationMode.Default} rules.");
            }
        }

        private static void ChooseStorage(string[] args)
        {
            int storageCommandIndex = Array.FindIndex(args, (arg) => arg.StartsWith(StorageFullCommand, StringComparison.InvariantCultureIgnoreCase));

            if (storageCommandIndex >= 0)
            {
                ProcessStorageFullCommand(args, storageCommandIndex);
                return;
            }

            storageCommandIndex = Array.FindIndex(args, (arg) => arg.Equals(StorageShortCommand, StringComparison.InvariantCultureIgnoreCase));

            if (storageCommandIndex >= 0)
            {
                ProcessStorageShortCommand(args, storageCommandIndex);
                return;
            }

            fileCabinetService = new FileCabinetMemoryService(chosenValidator);
        }

        private static void ProcessStorageFullCommand(string[] args, int storageCommandIndex)
        {
            var inputs = args[storageCommandIndex].Split(DefaultStartupModeSeparator, StringSplitOptions.TrimEntries);

            string inputStorageMode = inputs.Last();

            SetStorage(inputStorageMode);
        }

        private static void ProcessStorageShortCommand(string[] args, int storageCommandIndex)
        {
            if (storageCommandIndex == args.Length - 1)
            {
                fileCabinetService = new FileCabinetMemoryService(chosenValidator);

                return;
            }

            string inputStorageMode = args[storageCommandIndex + 1].Trim();

            SetStorage(inputStorageMode);
        }

        private static void SetStorage(string inputStorageMode)
        {
            int indexOfMode = Array.FindIndex(StorageSet, (rule) => rule.Item1.Equals(inputStorageMode, StringComparison.InvariantCultureIgnoreCase));

            if (indexOfMode >= 0)
            {
                switch (StorageSet[indexOfMode].Item2)
                {
                    case StorageMode.Memory: fileCabinetService = new FileCabinetMemoryService(chosenValidator); break;
                    case StorageMode.File: fileCabinetService = new FileCabinetFilesystemService(new FileStream(CabinetRecordsFile, FileMode.Create), chosenValidator); break;
                }
            }
            else
            {
                Console.WriteLine($"Unknown storage mode: '{inputStorageMode}'.");

                fileCabinetService = new FileCabinetMemoryService(chosenValidator);
            }
        }
    }
}