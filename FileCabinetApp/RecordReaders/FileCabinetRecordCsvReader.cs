using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace FileCabinetApp.RecordReaders
{
    /// <summary>
    /// Represents class for reading records from *.csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private const char PropertiesSeparator = ',';

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">StreamReader to *.csv file.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all records from *.csv file.
        /// </summary>
        /// <returns>List of readed records. </returns>
        public List<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> readedRecords = new List<FileCabinetRecord>();

            while (!this.reader.EndOfStream)
            {
                readedRecords.Add(this.ReadOneRecord());
            }

            return readedRecords;
        }

        private FileCabinetRecord ReadOneRecord()
        {
            FileCabinetRecord readedRecord = new FileCabinetRecord();

            string readedLine = this.reader.ReadLine().Trim();

            var readedPropertiesValues = readedLine.Split(PropertiesSeparator, StringSplitOptions.RemoveEmptyEntries);

            PropertyInfo[] properties = readedRecord.GetType().GetProperties();

            if (readedPropertiesValues.Length == properties.Length)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    try
                    {
                        properties[i].SetValue(readedRecord, Convert.ChangeType(readedPropertiesValues[i].Trim(), properties[i].PropertyType, Culture));
                    }
                    catch (FormatException)
                    {
                        return readedRecord;
                    }
                }
            }

            return readedRecord;
        }
    }
}
