using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for update command and update parameters.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "update";

        private const string SetKeyWord = "set ";
        private const string WhereKeyWord = "where ";
        private const string WherePartSplitSeparator = " and ";
        private const char SetPartSplitSeparator = ',';

        private const int PartsSplitCount = 2;
        private const int NamesValuesSplitCount = 2;

        private const char NameValueSeparator = '=';

        private static readonly char[] ValueTrimChars = { '\'', ' ' };

        private readonly Dictionary<PropertyInfo, object> setPropertiesValues = new Dictionary<PropertyInfo, object>();
        private readonly Dictionary<PropertyInfo, object> wherePropertiesValues = new Dictionary<PropertyInfo, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
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

        private static void DisplayUpdatedRecordsIds(List<int> updatedRecordsIds)
        {
            if (updatedRecordsIds.Count == 1)
            {
                Console.WriteLine($"Record #{updatedRecordsIds.First()} is updated.");
            }
            else
            {
                Console.Write("Records ");
                for (int i = 0; i < updatedRecordsIds.Count - 1; i++)
                {
                    Console.Write($"#{updatedRecordsIds[i]}, ");
                }

                Console.WriteLine($"#{updatedRecordsIds.Last()} are updated.");
            }
        }

        private static bool ProcessPartInputs(string[] partInputs, Dictionary<PropertyInfo, object> partDictionary, Type typeForProperties)
        {
            if (partInputs.Length == 0)
            {
                Console.WriteLine("One of the parameter-parts is empty.");
                return false;
            }

            foreach (var input in partInputs)
            {
                var propertyNamesValues = input.Split(NameValueSeparator, StringSplitOptions.RemoveEmptyEntries);

                if (propertyNamesValues.Length != NamesValuesSplitCount)
                {
                    Console.WriteLine($"Property should have one value");
                    return false;
                }

                string propertyName = propertyNamesValues.First().Trim();
                string propertyValue = propertyNamesValues.Last().Trim(ValueTrimChars);

                PropertyInfo property = GetProperty(propertyName, typeForProperties);

                if (property == null)
                {
                    Console.WriteLine($"There is no '{propertyName}' property for the record.");
                    return false;
                }

                object value;

                try
                {
                    value = Convert.ChangeType(propertyValue, property.PropertyType, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"{ex.Message} - {propertyName}='{propertyValue}'.");
                    return false;
                }

                partDictionary.Add(property, value);
            }

            return true;
        }

        private static PropertyInfo GetProperty(string inputPropertyName, Type type)
        {
            PropertyInfo property = Array.Find(type.GetProperties(), (property) => property.Name.Equals(inputPropertyName, StringComparison.InvariantCultureIgnoreCase));

            return property;
        }

        private void Insert(string parameters)
        {
            parameters = parameters.Trim();

            if (!parameters.StartsWith(SetKeyWord, StringComparison.InvariantCultureIgnoreCase) || !parameters.Contains(WhereKeyWord, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Update parameters should start with '{SetKeyWord}' word and contains '{WhereKeyWord}' word.");
                return;
            }

            parameters = parameters.Remove(default, SetKeyWord.Length).Trim();

            parameters = parameters.Replace(WhereKeyWord, WhereKeyWord, StringComparison.InvariantCultureIgnoreCase);

            var setAndWhereParts = parameters.Split(WhereKeyWord, StringSplitOptions.TrimEntries);

            if (setAndWhereParts.Length != PartsSplitCount)
            {
                Console.WriteLine($"There should be {PartsSplitCount} parts: {SetKeyWord} and {WhereKeyWord}.");
                return;
            }

            string setPart = setAndWhereParts.First();
            string wherePart = setAndWhereParts.Last();

            try
            {
                if (this.ProcessSetPart(setPart) && this.ProcessWherePart(wherePart))
                {
                    var findedRecords = this.Service.FindRecords(this.wherePropertiesValues, OperationType.And).ToList();

                    if (findedRecords.Count == 0)
                    {
                        Console.WriteLine("No records finded for update");
                        return;
                    }

                    this.UpdateFindedRecords(findedRecords);

                    DisplayUpdatedRecordsIds(findedRecords.Select((rec) => rec.Id).ToList());
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool ProcessSetPart(string setPart)
        {
            this.setPropertiesValues.Clear();

            if (setPart.Contains("id", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Set phase can not contain 'id' property to update.");
                return false;
            }

            var setInputs = setPart.Split(SetPartSplitSeparator, StringSplitOptions.RemoveEmptyEntries);

            return ProcessPartInputs(setInputs, this.setPropertiesValues, typeof(RecordParameters));
        }

        private bool ProcessWherePart(string wherePart)
        {
            this.wherePropertiesValues.Clear();

            wherePart = wherePart.Replace(WherePartSplitSeparator, WherePartSplitSeparator, StringComparison.InvariantCultureIgnoreCase);

            var whereInputs = wherePart.Split(WherePartSplitSeparator, StringSplitOptions.RemoveEmptyEntries);

            return ProcessPartInputs(whereInputs, this.wherePropertiesValues, typeof(FileCabinetRecord));
        }

        private void UpdateFindedRecords(List<FileCabinetRecord> findedRecords)
        {
            foreach (var record in findedRecords)
            {
                RecordParameters parameters = (RecordParameters)record;

                foreach (var propertyValue in this.setPropertiesValues)
                {
                    propertyValue.Key.SetValue(parameters, propertyValue.Value);
                }

                this.Service.EditRecord(record.Id, parameters);
            }
        }
    }
}
