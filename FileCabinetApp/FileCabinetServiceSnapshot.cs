using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

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
    }
}
