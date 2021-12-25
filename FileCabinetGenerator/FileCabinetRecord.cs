using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Provides properties of the record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets unique Identificator of the record.
        /// </summary>
        /// <value></value>
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
    }
}
