using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides properties of the record.
    /// </summary>
    [Serializable]
    [XmlType("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets unique Identificator of the record.
        /// </summary>
        /// <value></value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets first name of the person.
        /// </summary>
        /// <value></value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the person.
        /// </summary>
        /// <value></value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth of the person.
        /// </summary>
        /// <value></value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets height of the person.
        /// </summary>
        /// <value></value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets salary of the person.
        /// </summary>
        /// <value></value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets gender of the person.
        /// </summary>
        /// <value></value>
        public char Sex { get; set; }

        public static explicit operator RecordParameters(FileCabinetRecord record)
        {
            if (record == null)
            {
                return null;
            }

            RecordParameters parameters = new RecordParameters()
            {
                FirstName = record.FirstName,
                LastName = record.LastName,
                DateOfBirth = record.DateOfBirth,
                Height = record.Height,
                Salary = record.Salary,
                Sex = record.Sex,
            };

            return parameters;
        }

        /// <summary>
        /// Converts record to record parameters.
        /// </summary>
        /// <param name="record">Record to be converted.</param>
        /// <returns>Record parameters.</returns>
        public static RecordParameters ToRecordParameters(FileCabinetRecord record)
        {
            return (RecordParameters)record;
        }
    }
}
