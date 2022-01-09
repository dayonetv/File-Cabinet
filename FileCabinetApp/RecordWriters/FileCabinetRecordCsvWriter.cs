using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.RecordWriters
{
    /// <summary>
    /// Represents class for saving records to *.csv files.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private const string DateFormat = "d";

        private readonly TextWriter csvWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">writer to save info. </param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.csvWriter = writer;
        }

        /// <summary>
        /// Write property-values if the record to *.csv file.
        /// </summary>
        /// <param name="record">record to be saved. </param>
        /// <param name="isFirst">is it the first record. </param>
        public void Write(FileCabinetRecord record, bool isFirst)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (isFirst)
            {
                this.WritePropertyNames(record.GetType());
            }

            foreach (var recordProperty in typeof(FileCabinetRecord).GetProperties())
            {
                if (recordProperty.GetValue(record) is DateTime dateofBirth)
                {
                    this.csvWriter.Write($"{dateofBirth.ToString(DateFormat, CultureInfo.InvariantCulture)}, ");
                    continue;
                }

                this.csvWriter.Write($"{recordProperty.GetValue(record)}, ");
            }

            this.csvWriter.WriteLine();
            this.csvWriter.Flush();
        }

        private void WritePropertyNames(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                this.csvWriter.Write($"{property.Name}, ");
            }

            this.csvWriter.WriteLine();
        }
    }
}
