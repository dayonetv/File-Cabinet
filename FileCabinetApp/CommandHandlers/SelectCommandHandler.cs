using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Operator for union searching criterias.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// No uinion.
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
    /// Handler for select command and select parameters.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "select";

        private const string KeyWord = "where ";
        private const string AndWord = " and ";
        private const string OrWord = " or ";

        private const char SelectSeparator = ',';
        private const char PropertyValueSeparator = '=';
        private const string DateFormat = "d";

        private const int PropertyPlusValueCount = 2;

        private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly char[] ValueTrimChars = { '\'', ' ' };

        private readonly List<PropertyInfo> recordPropertiesToDisplay = new List<PropertyInfo>();
        private readonly Dictionary<PropertyInfo, object> propertyValuePairs = new Dictionary<PropertyInfo, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public SelectCommandHandler(IFileCabinetService service)
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

            if (request.Command.Equals(CommandName, Comparison))
            {
                this.Select(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static PropertyInfo GetProperty(string inputPropertyName, Type type)
        {
            PropertyInfo property = Array.Find(type.GetProperties(), (property) => property.Name.Equals(inputPropertyName, Comparison));

            return property;
        }

        private static void PrintFrame(List<int> maxLengthList)
        {
            Console.Write("+");

            for (int i = 0; i < maxLengthList.Count; i++)
            {
                Console.Write($"{new string('-', maxLengthList[i] + 2)}+");
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

            (bool success, OperationType operatorType) whereProcessingResult = this.ProcessWherePart(wherePart);

            if (this.ProcessSelectPart(selectPart) && whereProcessingResult.success)
            {
                var findedRecords = this.Service.FindRecords(this.propertyValuePairs, whereProcessingResult.operatorType);

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

            var selectProperties = selectPart.Split(SelectSeparator, StringSplitOptions.RemoveEmptyEntries);

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
            this.propertyValuePairs.Clear();

            if (string.IsNullOrEmpty(wherePart) || string.IsNullOrWhiteSpace(wherePart))
            {
                return (true, OperationType.None);
            }

            OperationType operatorType;

            if (!wherePart.Contains(OrWord, Comparison) && !wherePart.Contains(AndWord, Comparison))
            {
                operatorType = OperationType.None;
            }
            else if (wherePart.Contains(AndWord, Comparison) && !wherePart.Contains(OrWord, Comparison))
            {
                operatorType = OperationType.And;
            }
            else if (wherePart.Contains(OrWord, Comparison) && !wherePart.Contains(AndWord, Comparison))
            {
                operatorType = OperationType.Or;
            }
            else
            {
                Console.WriteLine($"Where part should contains only '{OperationType.And.ToString()}' words or only '{OperationType.Or.ToString()}' words.");
                return (false, OperationType.None);
            }

            wherePart = wherePart.Replace(operatorType.ToString(), operatorType.ToString(), Comparison);

            var wherePropertiesNamesValues = wherePart.Split(operatorType.ToString(), StringSplitOptions.RemoveEmptyEntries);

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
                    this.propertyValuePairs.Add(property, value);
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
            List<int> propertiesNamesAndValuesMaxLenght = new List<int>();

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
                Console.Write("{0," + maxLengthList[i].ToString(Culture) + "} | ", this.recordPropertiesToDisplay[i].Name);
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
