using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents class for reading records from *.xml files.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlSerializer xmlSerializer;
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Reader to read info. </param>
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
            FileCabinetRecord[] readedRecords = (FileCabinetRecord[])this.xmlSerializer.Deserialize(this.reader);
            return readedRecords.ToList();
        }
    }
}
