using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for export command and export paramaters.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private const int AmountOfExportParams = 2;

        private static readonly Tuple<string, Func<FileInfo, bool, string>>[] SavingModes = new Tuple<string, Func<FileInfo, bool, string>>[]
        {
            new Tuple<string, Func<FileInfo, bool, string>>("csv", WriteToCsv),
            new Tuple<string, Func<FileInfo, bool, string>>("xml", WriteToXml),
        };

        private static readonly Tuple<char, bool>[] Choices = new Tuple<char, bool>[]
        {
            new Tuple<char, bool>('Y', true),
            new Tuple<char, bool>('N', false),
        };

        /// <summary>
        /// Handles 'export' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
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

        private static Tuple<bool, string, char> CharConverter(string input)
        {
            input = input.Trim();
            bool result = char.TryParse(input, out char symbol);
            return new Tuple<bool, string, char>(result, input, symbol);
        }

        private static Tuple<bool, string> YesNoChoiceValidator(char inputChoice)
        {
            bool result = Array.FindIndex(Choices, choice => choice.Item1.ToString().Equals(inputChoice.ToString(), StringComparison.InvariantCultureIgnoreCase)) >= 0;
            return new Tuple<bool, string>(result, result ? "Valid" : "Choice can only be 'Y' or 'N'");
        }
    }
}
