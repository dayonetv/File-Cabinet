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
    public class ImportCommandHandler : CommandHandlerBase
    {
        private const int AmountOFImportParams = 2;

        private static readonly Tuple<string, Func<FileInfo, string>>[] ImportModes = new Tuple<string, Func<FileInfo, string>>[]
        {
            new Tuple<string, Func<FileInfo, string>>("csv", ImportFromCsv),
            new Tuple<string, Func<FileInfo, string>>("xml", ImportFromXml),
        };

        /// <summary>
        /// Handles 'import' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
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
    }
}
