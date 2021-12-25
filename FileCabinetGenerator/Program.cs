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

        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const int MaxSalary = 1_000_000;
        private const short MaxHeight = 220;
        private const short MinHeight = 120;
        private const string Chars = "abcdefghijklmnopqrstuvw";
        private const string DateFormat = "d";

        private static readonly char[] Genders = { 'M', 'F' };
        private static readonly DateTime MinDateOfBirth = new (1950, 1, 1);

        private static readonly Random Randomizer = new Random();

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

        private static readonly Tuple<string, Action<List<FileCabinetRecord>>>[] OutputFomatTypes = new Tuple<string, Action<List<FileCabinetRecord>>>[]
        {
            new Tuple<string, Action<List<FileCabinetRecord>>>("csv", WriteToCsv),
            new Tuple<string, Action<List<FileCabinetRecord>>>("xml", WriteToXml),
        };

        private static StreamWriter csvWriter;
        private static FileInfo targetFile;

        /// <summary>
        /// The main console-application entry point.
        /// </summary>
        /// <param name="args">Applicattion startup parameters. </param>
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine($"Invalid startup arguments");
                return;
            }

            var configuration = GetConfiguration(args);

            if (configuration.amount >= 0 && configuration.id >= 0 && configuration.file != null && configuration.writeToMethod != null)
            {
                targetFile = configuration.file;

                List<FileCabinetRecord> records = GenerateRandomRecords(configuration.id, configuration.amount);

                try
                {
                    configuration.writeToMethod?.Invoke(records);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                Console.WriteLine($"{configuration.amount} records were written to {configuration.file.FullName}");
            }
            else
            {
                Console.WriteLine("Wrong Startup Parameters");
            }
        }

        private static (int id, int amount, FileInfo file, Action<List<FileCabinetRecord>> writeToMethod) GetConfiguration(string[] args)
        {
            Action<List<FileCabinetRecord>> writeMethod = default;
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

        private static void ProcessCommand(int indexOfCommand, string parameter, ref int startId, ref FileInfo file, ref int amountOfRecords, ref Action<List<FileCabinetRecord>> writeMethod)
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

        private static Action<List<FileCabinetRecord>> GetWriteToMethod(string fileFormat)
        {
            int formatTypeIndex = Array.FindIndex(OutputFomatTypes, (tuple) => tuple.Item1.Equals(fileFormat, StringComparison.InvariantCultureIgnoreCase));

            if (formatTypeIndex >= 0)
            {
                return OutputFomatTypes[formatTypeIndex].Item2;
            }

            Console.WriteLine($"Wrong file format: {fileFormat}");
            return null;
        }

        private static void WriteToCsv(List<FileCabinetRecord> recordsToWrite)
        {
            csvWriter ??= new StreamWriter(targetFile.FullName);

            foreach (var record in recordsToWrite)
            {
                csvWriter.Write($"{record.Id},");
                csvWriter.Write($"{record.FirstName},");
                csvWriter.Write($"{record.LastName},");
                csvWriter.Write($"{record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture)},");
                csvWriter.Write($"{record.Height},");
                csvWriter.Write($"{record.Salary},");
                csvWriter.Write($"{record.Sex}");
                csvWriter.WriteLine();
            }

            csvWriter.Close();
        }

        private static void WriteToXml(List<FileCabinetRecord> recordsToWrite)
        {
            throw new NotImplementedException();
        }

        private static List<FileCabinetRecord> GenerateRandomRecords(int startId, int recordsAmount)
        {
            List<FileCabinetRecord> generatedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < recordsAmount; i++)
            {
                FileCabinetRecord randomRecord = new FileCabinetRecord()
                {
                    Id = startId++,
                    FirstName = GetRandomString(),
                    LastName = GetRandomString(),
                    DateOfBirth = GetRandomDateOfBirth(),
                    Height = GetRandomHeight(),
                    Salary = GetRandomSalary(),
                    Sex = GetRandomGender(),
                };

                generatedRecords.Add(randomRecord);
            }

            return generatedRecords;
        }

        private static string GetRandomString()
        {
            int stringLength = Randomizer.Next(MinNameLength, MaxNameLength);
            return new string(Enumerable.Repeat(Chars, stringLength).Select(s => s[Randomizer.Next(s.Length)]).ToArray());
        }

        private static short GetRandomHeight()
        {
            return (short)Randomizer.Next(MinHeight, MaxHeight);
        }

        private static char GetRandomGender()
        {
            return Genders[Randomizer.Next(Genders.Length)];
        }

        private static decimal GetRandomSalary()
        {
            return Randomizer.Next(0, MaxSalary);
        }

        private static DateTime GetRandomDateOfBirth()
        {
            int year = Randomizer.Next(MinDateOfBirth.Year, DateTime.Now.Year);
            int month = year == DateTime.Now.Year ? Randomizer.Next(MinDateOfBirth.Month, DateTime.Now.Month) : Randomizer.Next(MinDateOfBirth.Month, DateTime.MaxValue.Month);
            int day = year == DateTime.Now.Year && month == DateTime.Now.Month ? Randomizer.Next(MinDateOfBirth.Day, DateTime.Now.Day) : Randomizer.Next(MinDateOfBirth.Day, DateTime.DaysInMonth(year, month));
            return new DateTime(year, month, day);
        }
    }
}
