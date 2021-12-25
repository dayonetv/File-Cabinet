using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Generator of file cabinet reacords.
    /// </summary>
    public static class Program
    {
        private const int AmountOfStringsForShortCommand = 8;
        private const int AmountOfStringsForFullCommand = 4;
        private const char FullCommandSeparator = '=';

        private const int TypeFormatCommandIndex = 0;
        private const int FileNameCommandIndex = 1;
        private const int RecordsAmountCommandIndex = 2;
        private const int StartIdCommandIndex = 3;

        private static readonly string[] StartUpFullCommands = new string[]
        {
            "--output-type",
            "--output",
            "--records-amount",
            "--start-id",
        };

        private static readonly string[] StartUpShortCommands = new string[]
        {
            "-t",
            "-o",
            "-a",
            "-i",
        };

        private static readonly Tuple<string, Action<FileCabinetRecord>>[] OutputFomatTypes = new Tuple<string, Action<FileCabinetRecord>>[]
        {
            new Tuple<string, Action<FileCabinetRecord>>("csv", WriteToCsv),
            new Tuple<string, Action<FileCabinetRecord>>("xml", WriteToXml),
        };

        /// <summary>
        /// The main console-application entry point.
        /// </summary>
        /// <param name="args">Applicattion startup parameters. </param>
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine($"Invalid input args");
                return;
            }

            var result = ValidateInputArgs(args);

            Console.WriteLine($"{result.amount} were written to {result.file.FullName}");
        }

        private static (int id, int amount, FileInfo file, Action<FileCabinetRecord> writeToMethod) ValidateInputArgs(string[] args)
        {
            Action<FileCabinetRecord> writeMethod = default;
            FileInfo file = default;
            int startId = -1;
            int amountOfRecords = -1;

            if (args.Length == AmountOfStringsForFullCommand)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var inputs = args[i].Split(FullCommandSeparator, 2);

                    string command = inputs.First();
                    string parameter = inputs[^1];

                    int indexOfCommand = Array.FindIndex(StartUpFullCommands, (cmd) => cmd.Equals(command, StringComparison.InvariantCultureIgnoreCase));

                    if (indexOfCommand >= 0)
                    {
                        ProcessCommand(indexOfCommand, parameter, ref startId, ref file, ref amountOfRecords, ref writeMethod);
                    }
                    else
                    {
                        Console.WriteLine($"Unkown command {command}");
                        break;
                    }
                }
            }
            else if (args.Length == AmountOfStringsForShortCommand)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string command = args[i];
                    string parameter = args[++i];

                    int indexOfCommand = Array.FindIndex(StartUpShortCommands, (cmd) => cmd.Equals(command, StringComparison.InvariantCultureIgnoreCase));

                    if (indexOfCommand >= 0)
                    {
                        ProcessCommand(indexOfCommand, parameter, ref startId, ref file, ref amountOfRecords, ref writeMethod);
                    }
                    else
                    {
                        Console.WriteLine($"Unkown command {command}");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong amount of commands");
            }

            return (startId, amountOfRecords, file, writeMethod);
        }

        private static void ProcessCommand(int indexOfCommand, string parameter, ref int startId, ref FileInfo file, ref int amountOfRecords, ref Action<FileCabinetRecord> writeMethod)
        {
            switch (indexOfCommand)
            {
                case TypeFormatCommandIndex: writeMethod = GetWriteToMethod(parameter); return;
                case FileNameCommandIndex: file = GetFile(parameter); return;
                case RecordsAmountCommandIndex: amountOfRecords = GetAmountOfRecords(parameter); return;
                case StartIdCommandIndex: startId = GetStartId(parameter); return;
                default: return;
            }
        }

        private static int GetStartId(string id)
        {
            if (int.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out int startId))
            {
                return startId;
            }

            Console.WriteLine($"Wrong start id format: {id}");
            return -1;
        }

        private static int GetAmountOfRecords(string amount)
        {
            if (int.TryParse(amount, NumberStyles.None, CultureInfo.InvariantCulture, out int amountOfrecords))
            {
                return amountOfrecords;
            }

            Console.WriteLine($"Wrong amount format: {amount}");
            return -1;
        }

        private static FileInfo GetFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            if (Directory.Exists(file.DirectoryName))
            {
                return file;
            }

            Console.WriteLine($"Can not find the directory: {filePath}");
            return null;
        }

        private static Action<FileCabinetRecord> GetWriteToMethod(string fileFormat)
        {
            int formatTypeIndex = Array.FindIndex(OutputFomatTypes, (tuple) => tuple.Item1.Equals(fileFormat, StringComparison.InvariantCultureIgnoreCase));

            if (formatTypeIndex >= 0)
            {
                return OutputFomatTypes[formatTypeIndex].Item2;
            }

            Console.WriteLine($"Wrong file format: {fileFormat}");
            return null;
        }

        private static void WriteToCsv(FileCabinetRecord recordToWrite)
        {
            throw new NotImplementedException();
        }

        private static void WriteToXml(FileCabinetRecord recordToWrite)
        {
            throw new NotImplementedException();
        }

        private static List<FileCabinetRecord> GenerateRandomRecords(int startId, int recordsAmount)
        {
            throw new NotImplementedException();
        }

        private static string GetRandomString()
        {
            throw new NotImplementedException();
        }

        private static short GetRandomHeight()
        {
            throw new NotImplementedException();
        }

        private static char GetRandomGender()
        {
            throw new NotImplementedException();
        }

        private static decimal GetRandomSalary()
        {
            throw new NotImplementedException();
        }

        private static DateTime GetRandomDateOfBirth()
        {
            throw new NotImplementedException();
        }
    }
}
