using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for delete command and delete parameters.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "delete";
        private const string KeyWord = "where";
        private const char Separator = '=';
        private const char ValueTrimChar = '\'';
        private const int SplitAmount = 1;
        private const int NameValueSplitAmount = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
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
                this.Delete(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static PropertyInfo GetProperty(string inputPropertyName)
        {
            PropertyInfo property = Array.Find(typeof(FileCabinetRecord).GetProperties(), (property) => property.Name.Equals(inputPropertyName, StringComparison.InvariantCultureIgnoreCase));

            return property;
        }

        private static void DisplayDeletedIds(List<int> deletedIds)
        {
            if (deletedIds.Count == 0)
            {
                Console.WriteLine("No records deleted.");
            }
            else if (deletedIds.Count == 1)
            {
                Console.WriteLine($"Record #{deletedIds.First()} is deleted.");
            }
            else
            {
                Console.Write("Records ");
                for (int i = 0; i < deletedIds.Count - 1; i++)
                {
                    Console.Write($"#{deletedIds[i]}, ");
                }

                Console.WriteLine($"#{deletedIds.Last()} are deleted.");
            }
        }

        private void Delete(string parameters)
        {
            parameters = parameters.Trim();

            if (!parameters.StartsWith(KeyWord, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Delete parameters should start with '{KeyWord}' word.");
                return;
            }

            parameters = parameters.Replace(KeyWord, KeyWord, StringComparison.InvariantCultureIgnoreCase);

            var inputs = parameters.Split(KeyWord, StringSplitOptions.RemoveEmptyEntries);

            if (inputs.Length != SplitAmount)
            {
                Console.WriteLine($"Wrong amount of parameters after '{KeyWord}' word.");
                return;
            }

            var propertyWithValue = inputs.First().Split(Separator, StringSplitOptions.TrimEntries);

            if (propertyWithValue.Length != NameValueSplitAmount)
            {
                Console.WriteLine($"There should be one '{Separator}' symbol after {KeyWord} word.");
                return;
            }

            string propertyName = propertyWithValue[0].Trim();
            string propertyValue = propertyWithValue[^1].Trim(ValueTrimChar);

            PropertyInfo property = GetProperty(propertyName);

            if (property == null)
            {
                Console.WriteLine($"There is no '{propertyName}' property for the record.");
                return;
            }

            try
            {
                object value = Convert.ChangeType(propertyValue, property.PropertyType, CultureInfo.InvariantCulture);
                var deletedIds = this.Service.Delete(property, value);
                DisplayDeletedIds(deletedIds);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid format for {propertyName} value. {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
