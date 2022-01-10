using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp.RecordReaders
{
    /// <summary>
    /// Represents class for reading records from *.xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlSerializer xmlSerializer;
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">StreamReader to *.xml file. </param>
        public FileCabinetRecordXmlReader(FileStream reader)
        {
            this.reader = XmlReader.Create(reader);
            this.xmlSerializer = new XmlSerializer(typeof(FileCabinetRecord[]));
        }

        /// <summary>
        /// Reads all records from *.xml file.
        /// </summary>
        /// <returns>List of readed records. </returns>
        public List<FileCabinetRecord> ReadAll()
        {
            FileCabinetRecord[] readedRecords = Array.Empty<FileCabinetRecord>();

            try
            {
                readedRecords = (FileCabinetRecord[])this.xmlSerializer.Deserialize(this.reader);
            }
            catch (InvalidCastException)
            {
                return readedRecords.ToList();
            }
            catch (InvalidOperationException)
            {
                return readedRecords.ToList();
            }

            return readedRecords.ToList();
        }
    }
}
