using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameters object of the record.
    /// </summary>
    public class RecordParameters
    {
        /// <summary>
        /// Gets or sets first name parameter.
        /// </summary>
        /// <value><see cref="string"/> Firstname.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name parameter.
        /// </summary>
        /// <value><see cref="string"/> Lastname.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth parameter.
        /// </summary>
        /// <value><see cref="DateTime"/> date of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets height parameter.
        /// </summary>
        /// <value><see cref="short"/> height in centimeters.</value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets salary parameter.
        /// </summary>
        /// <value><see cref="decimal"/> salary value.</value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets gender parameter.
        /// </summary>
        /// <value><see cref="char"/> that represents gender.</value>
        public char Sex { get; set; }
    }
}
