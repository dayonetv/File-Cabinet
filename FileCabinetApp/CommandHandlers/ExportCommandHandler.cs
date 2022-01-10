using System;
using System.IO;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for 'export' command and paramaters.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "export";

        private const int AmountOfExportParams = 2;

        private static readonly Tuple<char, bool>[] Choices = new Tuple<char, bool>[]
        {
            new Tuple<char, bool>('Y', true),
            new Tuple<char, bool>('N', false),
        };

        private readonly Tuple<string, Func<FileInfo, bool, string>>[] savingModes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.savingModes = new Tuple<string, Func<FileInfo, bool, string>>[]
            {
                new Tuple<string, Func<FileInfo, bool, string>>("csv", this.WriteToCsv),
                new Tuple<string, Func<FileInfo, bool, string>>("xml", this.WriteToXml),
            };
        }

        /// <summary>
        /// Handles 'export' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        /// <exception cref="ArgumentNullException">request is null.</exception>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Export(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static Tuple<bool, string> YesNoChoiceValidator(char inputChoice)
        {
            bool result = Array.FindIndex(Choices, choice => choice.Item1.ToString().Equals(inputChoice.ToString(), StringComparison.InvariantCultureIgnoreCase)) >= 0;

            return new Tuple<bool, string>(result, result ? "Valid" : "Choice can only be 'Y' or 'N'");
        }

        private void Export(string parameters)
        {
            var inputParams = parameters.Trim().Split(' ', AmountOfExportParams);

            if (inputParams.Length != AmountOfExportParams)
            {
                Console.WriteLine($"'export' command requires at least {AmountOfExportParams} parameters. ");
                return;
            }

            string fileExtention = inputParams[0].Trim();
            string fileName = inputParams[^1].Trim();

            FileInfo exportFile = new (fileName);

            bool toRewrite = true;

            if (exportFile.Exists)
            {
                Console.WriteLine($"File is exist - rewrite {exportFile.FullName}? [Y/n] ");
                char inputChoice = ReadInput(CharConverter, YesNoChoiceValidator);

                int choiseIndex = Array.FindIndex(Choices, choice => choice.Item1.ToString().Equals(inputChoice.ToString(), StringComparison.InvariantCultureIgnoreCase));
                toRewrite = Choices[choiseIndex].Item2;
            }

            int saveModeIndex = Array.FindIndex(this.savingModes, (mode) => mode.Item1.Equals(fileExtention, StringComparison.InvariantCultureIgnoreCase));

            if (saveModeIndex >= 0)
            {
                try
                {
                    Console.WriteLine(this.savingModes[saveModeIndex].Item2.Invoke(exportFile, toRewrite));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Unknown: '{fileExtention}' parameter for 'export' command");
            }
        }

        private string WriteToCsv(FileInfo fileToWriteTo, bool rewrite)
        {
            if (rewrite)
            {
                StreamWriter writer = fileToWriteTo.CreateText();

                var snapShot = this.Service.MakeSnapShot();
                snapShot.SaveToScv(writer);

                writer.Close();

                return $"All records are exported to file {fileToWriteTo.FullName}.";
            }

            return $"Saving canceled";
        }

        private string WriteToXml(FileInfo fileToWriteTo, bool rewrite)
        {
            if (rewrite)
            {
                StreamWriter writer = fileToWriteTo.CreateText();

                var snapShot = this.Service.MakeSnapShot();
                snapShot.SaveToXml(writer);

                writer.Close();

                return $"All records are exported to file {fileToWriteTo.FullName}.";
            }

            return $"Saving canceled";
        }
    }
}
