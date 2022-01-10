using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using FileCabinetApp.RecordReaders;
using FileCabinetApp.RecordWriters;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents snapshot of current properties-values of the records.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Collection of snapshoted records. </param>
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
        /// <value>Collection of readed records.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; private set; }

        /// <summary>
        /// Saves all records to *.csv file.
        /// </summary>
        /// <param name="writer">Stream to *.csv file.</param>
        public void SaveToScv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter csvWriter = new (writer);

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i], i == 0);
            }
        }

        /// <summary>
        /// Saves all records to *.xml file.
        /// </summary>
        /// <param name="writer">Stream to *.xml file.</param>
        public void SaveToXml(StreamWriter writer)
        {
            FileCabinetRecordXmlWriter xmlWriter = new (XmlWriter.Create(writer));

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
            FileCabinetRecordCsvReader csvReader = new (reader);

            this.Records = csvReader.ReadAll().AsReadOnly();
        }

        /// <summary>
        /// Reads all records from *.xml file.
        /// </summary>
        /// <param name="reader">Stream to *.xml file.</param>
        public void LoadFromXml(FileStream reader)
        {
            FileCabinetRecordXmlReader xmlReader = new (reader);

            this.Records = xmlReader.ReadAll().AsReadOnly();
        }
    }
}
