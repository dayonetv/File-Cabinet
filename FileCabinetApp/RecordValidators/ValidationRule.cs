using System.Collections.Generic;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validation rule properties for json file.
    /// </summary>
    public class ValidationRule
    {
        /// <summary>
        /// Gets valid firstname length range.
        /// </summary>
        /// <value></value>
        public Range FirstName { get; init; }

        /// <summary>
        /// Gets valid lastname length range.
        /// </summary>
        /// <value></value>
        public Range LastName { get; init; }

        /// <summary>
        /// Gets valid date of birth range.
        /// </summary>
        /// <value></value>
        public DateRange DateOfBirth { get; init; }

        /// <summary>
        /// Gets valid salary range.
        /// </summary>
        /// <value></value>
        public Range Salary { get; init; }

        /// <summary>
        /// Gets valid height range.
        /// </summary>
        /// <value></value>
        public Range Height { get; init; }

        /// <summary>
        /// Gets valid gerders.
        /// </summary>
        /// <value></value>
        public List<char> Genders { get; init; }
    }
}
