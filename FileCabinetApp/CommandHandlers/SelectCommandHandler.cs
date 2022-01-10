using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Logical operator for searching criteria.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// Without operator.
        /// </summary>
        None,

        /// <summary>
        /// Logical 'and'.
        /// </summary>
        And,

        /// <summary>
        /// Logical 'or'.
        /// </summary>
        Or,
    }

    /// <summary>
    /// Handler for 'select' command and parameters.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "select";

        private const string KeyWord = "where ";
        private const string AndWord = " and ";
        private const string OrWord = " or ";

        private const char SelectPropertiesSeparator = ',';
        private const char PropertyValueSeparator = '=';
        private const string DateFormat = "d";

        private const int PropertyPlusValueCount = 2;

        private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly char[] ValueTrimChars = { '\'', ' ' };

        private readonly List<PropertyInfo> recordPropertiesToDisplay = new ();
        private readonly Dictionary<PropertyInfo, object> propertyNameValuePairs = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'select' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        /// <exception cref="ArgumentNullException">request is null.</exception>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command.Equals(CommandName, Comparison))
            {
                this.Select(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static string[] GetPropertiesNamesAndValues(string wherePart, out OperationType operatorType, out bool correctness)
        {
            if (!wherePart.Contains(OrWord, Comparison) && !wherePart.Contains(AndWord, Comparison))
            {
                operatorType = OperationType.None;
                correctness = true;

                return new string[] { wherePart };
            }
            else if (wherePart.Contains(AndWord, Comparison) && !wherePart.Contains(OrWord, Comparison))
            {
                operatorType = OperationType.And;
                wherePart = wherePart.Replace(AndWord, AndWord, Comparison);
                correctness = true;

                return wherePart.Split(AndWord, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (wherePart.Contains(OrWord, Comparison) && !wherePart.Contains(AndWord, Comparison))
            {
                operatorType = OperationType.Or;
                wherePart = wherePart.Replace(OrWord, OrWord, Comparison);
                correctness = true;

                return wherePart.Split(OrWord, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                Console.WriteLine($"Where part should contains only '{OperationType.And}' words or only '{OperationType.Or}' words.");

                operatorType = OperationType.None;
                correctness = false;

                return Array.Empty<string>();
            }
        }

        private static void PrintFrame(List<int> maxLengthList)
        {
            const int WhiteSpaceAmount = 2;

            Console.Write("+");

            for (int i = 0; i < maxLengthList.Count; i++)
            {
                Console.Write($"{new string('-', maxLengthList[i] + WhiteSpaceAmount)}+");
            }

            Console.WriteLine();
        }

        private void Select(string parameters)
        {
            parameters = parameters.Trim();
            parameters = parameters.Replace(KeyWord, KeyWord, Comparison);

            var inputs = parameters.Split(KeyWord, StringSplitOptions.TrimEntries);

            string selectPart = inputs.First();
            string wherePart = inputs.Last() != selectPart ? inputs.Last() : string.Empty;

            (bool wherePartCorectness, OperationType operatorType) = this.ProcessWherePart(wherePart);

            if (wherePartCorectness && this.ProcessSelectPart(selectPart))
            {
                var findedRecords = this.Service.FindRecords(this.propertyNameValuePairs, operatorType);

                this.DisplayRecords(findedRecords.ToList());
            }
        }

        private bool ProcessSelectPart(string selectPart)
        {
            this.recordPropertiesToDisplay.Clear();

            if (string.IsNullOrEmpty(selectPart) || string.IsNullOrWhiteSpace(selectPart))
            {
                this.recordPropertiesToDisplay.AddRange(typeof(FileCabinetRecord).GetProperties());
                return true;
            }

            var selectProperties = selectPart.Split(SelectPropertiesSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (selectProperties.Length == 0)
            {
                Console.WriteLine("Selected properties are empty.");
                return false;
            }

            foreach (var propertyName in selectProperties)
            {
                PropertyInfo property = GetProperty(propertyName.Trim(), typeof(FileCabinetRecord));

                if (property == null)
                {
                    Console.WriteLine($"There is no '{propertyName}' property for the record.");
                    return false;
                }

                if (this.recordPropertiesToDisplay.Contains(property))
                {
                    Console.WriteLine($"Property {property.Name} is already entered.");
                    return false;
                }

                this.recordPropertiesToDisplay.Add(property);
            }

            return true;
        }

        private (bool success, OperationType operatorType) ProcessWherePart(string wherePart)
        {
            this.propertyNameValuePairs.Clear();

            if (string.IsNullOrEmpty(wherePart) || string.IsNullOrWhiteSpace(wherePart))
            {
                return (true, OperationType.None);
            }

            var wherePropertiesNamesValues = GetPropertiesNamesAndValues(wherePart, out OperationType operatorType, out bool success);

            if (!success)
            {
                return (success, operatorType);
            }

            if (wherePropertiesNamesValues.Length == 0)
            {
                Console.WriteLine("Where properties-values are empty.");
                return (false, OperationType.None);
            }

            foreach (var propertyNameValue in wherePropertiesNamesValues)
            {
                var inputs = propertyNameValue.Split(PropertyValueSeparator, StringSplitOptions.RemoveEmptyEntries);

                if (inputs.Length != PropertyPlusValueCount)
                {
                    Console.WriteLine($"Properties and values should be separated with one '{PropertyValueSeparator}' char.");

                    return (false, OperationType.None);
                }

                string propertyName = inputs.First().Trim();
                string propertyValue = inputs.Last().Trim(ValueTrimChars);

                PropertyInfo property = GetProperty(propertyName, typeof(FileCabinetRecord));

                if (property == null)
                {
                    Console.WriteLine($"There is no '{propertyName}' property for the record.");
                    return (false, OperationType.None);
                }

                try
                {
                    object value = Convert.ChangeType(propertyValue, property.PropertyType, Culture);
                    this.propertyNameValuePairs.Add(property, value);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"{ex.Message} - {propertyName}='{propertyValue}'.");
                    return (false, OperationType.None);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Property {property.Name} is already entered. {ex.Message}");
                    return (false, OperationType.None);
                }
            }

            return (true, operatorType);
        }

        private void DisplayRecords(List<FileCabinetRecord> findedRecords)
        {
            if (!findedRecords.Any())
            {
                Console.WriteLine("No records selected.");
                return;
            }

            var maxLengthList = this.GetMaxLenghtList(findedRecords);

            PrintFrame(maxLengthList);
            this.PrintPropertyNames(maxLengthList);
            PrintFrame(maxLengthList);

            foreach (var record in findedRecords)
            {
                this.PrintRecord(record, maxLengthList);
            }

            PrintFrame(maxLengthList);
        }

        private List<int> GetMaxLenghtList(List<FileCabinetRecord> findedRecords)
        {
            List<int> propertiesNamesAndValuesMaxLenght = new ();

            foreach (var property in this.recordPropertiesToDisplay)
            {
                int maxLength = (from record in findedRecords select property.GetValue(record) is DateTime date ? date.ToString(DateFormat, Culture).Length : property.GetValue(record).ToString().Length).Max();

                propertiesNamesAndValuesMaxLenght.Add(maxLength > property.Name.Length ? maxLength : property.Name.Length);
            }

            return propertiesNamesAndValuesMaxLenght;
        }

        private void PrintPropertyNames(List<int> maxLengthList)
        {
            Console.Write("| ");

            for (int i = 0; i < maxLengthList.Count; i++)
            {
                Console.Write("{0,-" + maxLengthList[i].ToString(Culture) + "} | ", this.recordPropertiesToDisplay[i].Name);
            }

            Console.WriteLine();
        }

        private void PrintRecord(FileCabinetRecord recordToPrint, List<int> maxLengthList)
        {
            Console.Write("| ");

            for (int i = 0; i < maxLengthList.Count; i++)
            {
                if (this.recordPropertiesToDisplay[i].GetValue(recordToPrint) is DateTime date)
                {
                    Console.Write("{0," + maxLengthList[i].ToString(Culture) + "} | ", date.ToString(DateFormat, Culture));
                    continue;
                }

                if (this.recordPropertiesToDisplay[i].GetValue(recordToPrint) is string || this.recordPropertiesToDisplay[i].GetValue(recordToPrint) is char)
                {
                    Console.Write("{0,-" + maxLengthList[i].ToString(Culture) + "} | ", this.recordPropertiesToDisplay[i].GetValue(recordToPrint));
                    continue;
                }

                Console.Write("{0," + maxLengthList[i].ToString(Culture) + "} | ", this.recordPropertiesToDisplay[i].GetValue(recordToPrint));
            }

            Console.WriteLine();
        }
    }
}
