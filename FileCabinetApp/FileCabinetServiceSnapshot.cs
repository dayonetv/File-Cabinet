using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using FileCabinetApp.RecordReaders;
using FileCabinetApp.RecordWriters;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents snapshot of current property-values of the records.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">array of records to be snapshoted. </param>
        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
        }

        /// <summary>
        /// Gets readed records.
        /// </summary>
        /// <value>Readed records. </value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; private set; }

        /// <summary>
        /// Saves all records to *.scv file.
        /// </summary>
        /// <param name="writer">Stream to file.</param>
        public void SaveToScv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i], i == 0);
            }
        }

        /// <summary>
        /// Saves all records to *.xml file.
        /// </summary>
        /// <param name="writer">Stream to file.</param>
        public void SaveToXml(StreamWriter writer)
        {
            FileCabinetRecordXmlWriter xmlWriter = new FileCabinetRecordXmlWriter(XmlWriter.Create(writer));

            for (int i = 0; i < this.records.Length; i++)
            {
                xmlWriter.Write(this.records[i], i == 0, i == this.records.Length - 1);
            }
        }

        /// <summary>
        /// Reads all records from *.csv file.
        /// </summary>
        /// <param name="reader">Stream to *.csv file.</param>
        public void LoadFromScv(StreamReader reader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader);

            this.Records = csvReader.ReadAll().AsReadOnly();
        }

        /// <summary>
        /// Reads all records from *.xml file.
        /// </summary>
        /// <param name="reader">Stream to *.xml file.</param>
        public void LoadFromXml(FileStream reader)
        {
            FileCabinetRecordXmlReader xmlReader = new FileCabinetRecordXmlReader(reader);

            this.Records = xmlReader.ReadAll().AsReadOnly();
        }
    }
}
