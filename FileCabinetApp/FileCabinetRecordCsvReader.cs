using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents class for reading records from *.csv files.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Reader to read info. </param>
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

            string readedLine = this.reader.ReadLine();

            var properties = readedLine.Split(',');

            if (properties.Length == readedRecord.GetType().GetProperties().Length)
            {
                readedRecord.Id = int.Parse(properties[0], Culture);

                readedRecord.FirstName = properties[1].Trim();

                readedRecord.LastName = properties[2].Trim();

                readedRecord.DateOfBirth = DateTime.Parse(properties[3], Culture, DateTimeStyles.None);

                readedRecord.Height = short.Parse(properties[4], Culture);

                readedRecord.Salary = decimal.Parse(properties[5], Culture);

                readedRecord.Sex = char.Parse(properties[6]);
            }

            return readedRecord;
        }
    }
}
