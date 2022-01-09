using System;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp.RecordWriters
{
    /// <summary>
    /// Represents class for saving records to *.xml files.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private const string DateFormat = "d";

        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">writer to save info. </param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.xmlWriter = writer;
        }

        /// <summary>
        /// Write property-values if the record to *.xml file.
        /// </summary>
        /// <param name="record">record to be saved. </param>
        /// <param name="isFirst">is it the first record. </param>
        /// <param name="isLast">is it the last record. </param>
        public void Write(FileCabinetRecord record, bool isFirst, bool isLast)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (isFirst)
            {
                this.xmlWriter.WriteStartElement("records");
            }

            this.xmlWriter.WriteStartElement("record");
            this.xmlWriter.WriteAttributeString(nameof(record.Id), record.Id.ToString(CultureInfo.InvariantCulture));

            this.xmlWriter.WriteStartElement("name");
            this.xmlWriter.WriteAttributeString("last", record.LastName);
            this.xmlWriter.WriteAttributeString("first", record.FirstName);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement(nameof(record.DateOfBirth));
            this.xmlWriter.WriteValue(record.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture));
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement(nameof(record.Height));
            this.xmlWriter.WriteValue(record.Height);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement(nameof(record.Salary));
            this.xmlWriter.WriteValue(record.Salary);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement(nameof(record.Sex));
            this.xmlWriter.WriteValue(record.Sex);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteEndElement();

            if (isLast)
            {
                this.xmlWriter.WriteEndElement();
                this.xmlWriter.Flush();
            }
        }
    }
}
