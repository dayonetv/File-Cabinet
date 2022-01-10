using System.Collections.Generic;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Represents validation rule properties.
    /// </summary>
    public class ValidationRule
    {
        /// <summary>
        /// Gets valid FirsTname length range.
        /// </summary>
        /// <value><see cref="Range"/> for FirstName length property.</value>
        public Range FirstName { get; init; }

        /// <summary>
        /// Gets valid LastName length range.
        /// </summary>
        /// <value><see cref="Range"/> for LastName length property.</value>
        public Range LastName { get; init; }

        /// <summary>
        /// Gets valid DateOfBirth range.
        /// </summary>
        /// <value><see cref="DateRange"/> for DateOfBirth property.</value>
        public DateRange DateOfBirth { get; init; }

        /// <summary>
        /// Gets valid salary range.
        /// </summary>
        /// <value><see cref="Range"/> for Salary property.</value>
        public Range Salary { get; init; }

        /// <summary>
        /// Gets valid height range.
        /// </summary>
        /// <value><see cref="Range"/> for Height property.</value>
        public Range Height { get; init; }

        /// <summary>
        /// Gets valid gerders.
        /// </summary>
        /// <value>List of valid genders for sex property.</value>
        public List<char> Genders { get; init; }
    }
}
