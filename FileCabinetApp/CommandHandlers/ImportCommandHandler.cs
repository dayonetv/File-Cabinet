using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for import command and import paramaters.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const int AmountOFImportParams = 2;

        private readonly Tuple<string, Func<FileInfo, string>>[] importModes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.importModes = new Tuple<string, Func<FileInfo, string>>[]
            {
            new Tuple<string, Func<FileInfo, string>>("csv", this.ImportFromCsv),
            new Tuple<string, Func<FileInfo, string>>("xml", this.ImportFromXml),
            };
        }

        /// <summary>
        /// Handles 'import' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private void Import(string parameters)
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
                int importModeIndex = Array.FindIndex(this.importModes, (tuple) => tuple.Item1.Equals(importMode, StringComparison.InvariantCultureIgnoreCase));
                if (importModeIndex >= 0)
                {
                    try
                    {
                        string message = this.importModes[importModeIndex].Item2?.Invoke(importFile);
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

        private string ImportFromCsv(FileInfo fileToImportFrom)
        {
            StreamReader csvReader = fileToImportFrom.OpenText();

            try
            {
                var snapshot = new FileCabinetServiceSnapshot();
                snapshot.LoadFromScv(csvReader);
                string restoringMessage = this.Service.Restore(snapshot);

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

        private string ImportFromXml(FileInfo fileToImportFrom)
        {
            FileStream xmlReader = new FileStream(fileToImportFrom.FullName, FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                var snapshot = new FileCabinetServiceSnapshot();
                snapshot.LoadFromXml(xmlReader);
                string restoringMessage = this.Service.Restore(snapshot);

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
    }
}
