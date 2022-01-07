﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for insert command and insert paramaters.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "insert";

        private const string KeyWord = "values";
        private const char Separator = ',';
        private const char ValuesTrimChar = '\'';
        private const int SplitAmount = 2;

        private static readonly char[] TrimChars = new char[] { '(', ')' };

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Insert(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static FileCabinetRecord CreateRecord(string[] propertyNames, string[] propertyValues)
        {
            FileCabinetRecord record = new FileCabinetRecord();

            PropertyInfo[] recordProperties = record.GetType().GetProperties();

            for (int i = 0; i < propertyNames.Length; i++)
            {
                var property = (from prop in recordProperties where prop.Name.Equals(propertyNames[i], StringComparison.InvariantCultureIgnoreCase) select prop).First();

                property.SetValue(record, Convert.ChangeType(propertyValues[i].Trim(ValuesTrimChar), property.PropertyType, CultureInfo.InvariantCulture));
            }

            return record;
        }

        private static bool CheckPropertyNames(string[] inputPropertyNames)
        {
            var recordPropertyNames = (from prop in typeof(FileCabinetRecord).GetProperties() select prop.Name).ToList();

            if (recordPropertyNames.Count != inputPropertyNames.Length)
            {
                Console.WriteLine($"Insertion property count is not equal to the record property count.");
                return false;
            }

            foreach (var inputProp in inputPropertyNames)
            {
                if (!recordPropertyNames.Contains(inputProp, StringComparer.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine($"There is no '{inputProp}' property for the record.");
                    return false;
                }
            }

            return true;
        }

        private void Insert(string parameters)
        {
            if (!parameters.Contains(KeyWord, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"'insert' command requires '{KeyWord}' word.");
                return;
            }

            parameters = parameters.Replace(KeyWord, KeyWord, StringComparison.InvariantCultureIgnoreCase);

            var inputs = parameters.Split(KeyWord, StringSplitOptions.TrimEntries);

            if (inputs.Length != SplitAmount)
            {
                Console.WriteLine($"'insert' command can have only one '{KeyWord}' word.");
                return;
            }

            var propertyNames = inputs[0].Trim(TrimChars).Split(Separator, StringSplitOptions.TrimEntries);
            var propertyValues = inputs[1].Trim(TrimChars).Split(Separator, StringSplitOptions.TrimEntries);

            if (propertyNames.Length != propertyValues.Length)
            {
                Console.WriteLine($"Amount of values: '{propertyValues.Length}' is not equal to the amount of insertion properties : '{propertyNames.Length}'.");
                return;
            }

            if (CheckPropertyNames(propertyNames))
            {
                try
                {
                    FileCabinetRecord recordToInsert = CreateRecord(propertyNames, propertyValues);
                    this.Service.Insert(recordToInsert);

                    Console.WriteLine($"Record #{recordToInsert.Id} inserted.");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Wrong values format: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Validation failed: {ex.Message}");
                }
            }
        }
    }
}
