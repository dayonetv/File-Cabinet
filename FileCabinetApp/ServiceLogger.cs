using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Service that saves methods execution information.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string DateFormat = "d";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly FileInfo LogFile = new FileInfo("logs.txt");

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
        public int CreateRecord(CreateEditParameters parameters)
        {
            this.WriteInputs(nameof(this.CreateRecord), CreateEditParametersToString(parameters));

            var result = this.service.CreateRecord(parameters);

            this.WriteOutputs(nameof(this.CreateRecord), result.ToString(Culture));

            return result;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, CreateEditParameters parameters)
        {
            this.WriteInputs(nameof(this.EditRecord), $"{nameof(id)} = '{id}', {CreateEditParametersToString(parameters)}");

            this.service.EditRecord(id, parameters);

            this.WriteOutputs(nameof(this.EditRecord), null);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth)
        {
            this.WriteInputs(nameof(this.FindByDateOfBith), $"{nameof(dateOfBirth)} = {dateOfBirth.ToString(DateFormat, Culture)}");

            var records = this.service.FindByDateOfBith(dateOfBirth);

            StringBuilder outputText = new StringBuilder();

            foreach (var rec in records)
            {
                outputText.Append(RecordToString(rec));
            }

            this.WriteOutputs(nameof(this.FindByDateOfBith), outputText.ToString());

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.WriteInputs(nameof(this.FindByFirstName), $"{nameof(firstName)} = {firstName}");

            var records = this.service.FindByFirstName(firstName);

            StringBuilder outputText = new StringBuilder();

            foreach (var rec in records)
            {
                outputText.Append(RecordToString(rec));
            }

            this.WriteOutputs(nameof(this.FindByFirstName), outputText.ToString());

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.WriteInputs(nameof(this.FindByLastName), $"LastName = '{lastName}'");

            var records = this.service.FindByLastName(lastName);

            StringBuilder outputText = new StringBuilder();

            foreach (var rec in records)
            {
                outputText.Append(RecordToString(rec));
            }

            this.WriteOutputs(nameof(this.FindByLastName), outputText.ToString());

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.WriteInputs(nameof(this.GetRecords), null);

            var records = this.service.GetRecords();

            StringBuilder outputText = new StringBuilder();

            foreach (var rec in records)
            {
                outputText.Append(RecordToString(rec));
            }

            this.WriteOutputs(nameof(this.GetRecords), outputText.ToString());

            return records;
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            this.WriteInputs(nameof(this.GetStat), null);

            var result = this.service.GetStat();

            this.WriteInputs(nameof(this.GetStat), result.ToString(Culture));

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
        public bool Remove(int id)
        {
            this.WriteInputs(nameof(this.Remove), $"{nameof(id)} = '{id}'");

            var result = this.service.Remove(id);

            this.WriteOutputs(nameof(this.Remove), result.ToString(Culture));

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

        private static string RecordToString(FileCabinetRecord record)
        {
            StringBuilder text = new StringBuilder();

            text.Append($"{nameof(record.Id)} = '{record.Id}', ");
            text.Append($"{nameof(record.FirstName)} = '{record.FirstName}', ");
            text.Append($"{nameof(record.LastName)} = '{record.LastName}', ");
            text.Append($"{nameof(record.DateOfBirth)} = '{record.DateOfBirth.ToString(DateFormat, Culture)}', ");
            text.Append($"{nameof(record.Height)} = '{record.Height}', ");
            text.Append($"{nameof(record.Salary)} = '{record.Salary}', ");
            text.Append($"{nameof(record.Sex)} = '{record.Sex}'\n");

            return text.ToString();
        }

        private static string CreateEditParametersToString(CreateEditParameters parameters)
        {
            StringBuilder text = new StringBuilder();

            text.Append($"{nameof(parameters.FirstName)} = '{parameters?.FirstName}', ");
            text.Append($"{nameof(parameters.LastName)} = '{parameters?.LastName}', ");
            text.Append($"{nameof(parameters.DateOfBirth)} = '{parameters?.DateOfBirth.ToString(DateFormat, Culture)}', ");
            text.Append($"{nameof(parameters.Height)} = '{parameters?.Height}', ");
            text.Append($"{nameof(parameters.Salary)} = '{parameters?.Salary}', ");
            text.Append($"{nameof(parameters.Sex)} = '{parameters?.Sex}'\n");

            return text.ToString();
        }

        private void WriteInputs(string methodName, string inputParameters)
        {
            this.logWriter.WriteLine($"{DateTime.Now.ToLongDateString()} - Calling {methodName}() with {inputParameters ?? "null"}");
        }

        private void WriteOutputs(string methodName, string returningParameters)
        {
            this.logWriter.WriteLine($"{DateTime.Now.ToLongDateString()} - {methodName}() returned '{returningParameters ?? "void"}'");
        }
    }
}
