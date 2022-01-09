using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameter object for FileCabinetService.CreateRecord and FileCabinetService.EditRecord methods.
    /// </summary>
    public class RecordParameters
    {
        /// <summary>
        /// Gets or sets first name parameter.
        /// </summary>
        /// <value></value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name parameter.
        /// </summary>
        /// <value></value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth parameter.
        /// </summary>
        /// <value></value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets height parameter.
        /// </summary>
        /// <value></value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets salary parameter.
        /// </summary>
        /// <value></value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets gender parameter.
        /// </summary>
        /// <value></value>
        public char Sex { get; set; }
    }
}
