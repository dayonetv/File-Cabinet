using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides properties for the record.
    /// </summary>
    [Serializable]
    [XmlType("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets unique Identificator of the record.
        /// </summary>
        /// <value><see cref="int"/> record identificator.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets first name of the person.
        /// </summary>
        /// <value><see cref="string"/> Firstname.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the person.
        /// </summary>
        /// <value><see cref="string"/> Lastname.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth of the person.
        /// </summary>
        /// <value><see cref="DateTime"/> date of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets height of the person.
        /// </summary>
        /// <value><see cref="short"/> height in centimeters.</value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets salary of the person.
        /// </summary>
        /// <value><see cref="decimal"/> salary value.</value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets gender of the person.
        /// </summary>
        /// <value><see cref="char"/> that represents gender.</value>
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
        /// Converts record to record parameters object.
        /// </summary>
        /// <param name="record">Record to be converted.</param>
        /// <returns>Record parameters object.</returns>
        public static RecordParameters ToRecordParameters(FileCabinetRecord record)
        {
            return (RecordParameters)record;
        }
    }
}
