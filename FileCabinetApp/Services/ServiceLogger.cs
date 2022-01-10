using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Service that saves <see cref="IFileCabinetService"/> methods execution time information.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string PropertyDateFormat = "d";
        private const string LogsDateFormat = "G";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly FileInfo LogFile = new ("logs.txt");

        private readonly TextWriter logWriter = LogFile.CreateText();
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service to be used.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters == null)
            {
                ArgumentNullException exception = new ArgumentNullException(nameof(parameters));

                this.WriteInputs(nameof(this.CreateRecord), exception.Message);

                throw exception;
            }

            this.WriteInputs(nameof(this.CreateRecord), RecordParametersToString(parameters));

            var result = this.service.CreateRecord(parameters);

            this.WriteOutputs(nameof(this.CreateRecord), result.ToString(Culture));

            return result;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters parameters)
        {
            if (parameters == null)
            {
                ArgumentNullException exception = new ArgumentNullException(nameof(parameters));

                this.WriteInputs(nameof(this.EditRecord), exception.Message);

                throw exception;
            }

            this.WriteInputs(nameof(this.EditRecord), $"{nameof(id)} = '{id}', {RecordParametersToString(parameters)}");

            this.service.EditRecord(id, parameters);

            this.WriteOutputs(nameof(this.EditRecord), null);
        }

        /// <inheritdoc/>
        public (int total, int deleted) GetStat()
        {
            this.WriteInputs(nameof(this.GetStat), null);

            var result = this.service.GetStat();

            this.WriteOutputs(nameof(this.GetStat), $"Total: {result.total}, Deleted: {result.deleted}");

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapShot()
        {
            this.WriteInputs(nameof(this.MakeSnapShot), null);

            var snapshot = this.service.MakeSnapShot();

            this.WriteOutputs(nameof(this.MakeSnapShot), null);

            return snapshot;
        }

        /// <inheritdoc/>
        public int Purge()
        {
            this.WriteInputs(nameof(this.Purge), null);

            var result = this.service.Purge();

            this.WriteOutputs(nameof(this.Purge), result.ToString(Culture));

            return result;
        }

        /// <inheritdoc/>
        public List<int> Delete(PropertyInfo recordProperty, object propertyValue)
        {
            this.WriteInputs(nameof(this.Delete), $"{recordProperty?.Name} = '{propertyValue}'");

            var result = this.service.Delete(recordProperty, propertyValue);

            this.WriteOutputs(nameof(this.Delete), string.Join(',', result));

            return result;
        }

        /// <inheritdoc/>
        public string Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.WriteInputs(nameof(this.Restore), snapshot?.ToString());

            var result = this.service.Restore(snapshot);

            this.WriteOutputs(nameof(this.Restore), result);

            return result;
        }

        /// <inheritdoc/>
        public void Insert(FileCabinetRecord recordToInsert)
        {
            if (recordToInsert == null)
            {
                this.WriteOutputs(nameof(this.Insert), new ArgumentNullException(nameof(recordToInsert)).Message);
                return;
            }

            this.WriteInputs(nameof(this.Insert), RecordToString(recordToInsert));

            this.service.Insert(recordToInsert);

            this.WriteOutputs(nameof(this.Insert), null);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation)
        {
            this.WriteInputs(nameof(this.FindRecords), $"{string.Join(string.Empty, propertiesWithValues)}, {operation}");

            var records = this.service.FindRecords(propertiesWithValues, operation);

            StringBuilder outputText = new ();

            if (records != null)
            {
                foreach (var rec in records)
                {
                    outputText.Append(RecordToString(rec));
                }
            }

            this.WriteOutputs(nameof(this.FindRecords), outputText.ToString());

            return records;
        }

        private static string RecordToString(FileCabinetRecord record)
        {
            StringBuilder recordPropertiesAndValues = new ();

            PropertyInfo[] recordProperties = record.GetType().GetProperties();

            for (int i = 0; i < recordProperties.Length - 1; i++)
            {
                if (recordProperties[i].GetValue(record) is DateTime dateOfBirth)
                {
                    recordPropertiesAndValues.Append($"{recordProperties[i].Name} = '{dateOfBirth.ToString(PropertyDateFormat, Culture)}', ");
                    continue;
                }

                recordPropertiesAndValues.Append($"{recordProperties[i].Name} = '{recordProperties[i].GetValue(record)}', ");
            }

            recordPropertiesAndValues.Append($"{recordProperties[^1].Name} = '{recordProperties[^1].GetValue(record)}'; ");

            return recordPropertiesAndValues.ToString();
        }

        private static string RecordParametersToString(RecordParameters parameters)
        {
            StringBuilder parametersPropertiesAndValues = new ();

            PropertyInfo[] parametersProperties = parameters.GetType().GetProperties();

            for (int i = 0; i < parametersProperties.Length - 1; i++)
            {
                if (parametersProperties[i].GetValue(parameters) is DateTime dateOfBirth)
                {
                    parametersPropertiesAndValues.Append($"{parametersProperties[i].Name} = '{dateOfBirth.ToString(PropertyDateFormat, Culture)}', ");
                }

                parametersPropertiesAndValues.Append($"{parametersProperties[i].Name} = '{parametersProperties[i].GetValue(parameters)}', ");
            }

            parametersPropertiesAndValues.Append($"{parametersProperties[^1].Name} = '{parametersProperties[^1].GetValue(parameters)}'");

            return parametersPropertiesAndValues.ToString();
        }

        private void WriteInputs(string methodName, string inputParameters)
        {
            this.logWriter.WriteLine($"{DateTime.Now.ToString(LogsDateFormat, Culture)} - Calling {methodName}() with {inputParameters ?? "null"}");
            this.logWriter.Flush();
        }

        private void WriteOutputs(string methodName, string returningParameters)
        {
            this.logWriter.WriteLine($"{DateTime.Now.ToString(LogsDateFormat, Culture)} - {methodName}() returned '{returningParameters ?? "void"}'");
            this.logWriter.Flush();
        }
    }
}
