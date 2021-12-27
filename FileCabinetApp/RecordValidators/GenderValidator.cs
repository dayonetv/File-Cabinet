using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for gender property validation.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char[] validGenders;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="validGenders">Array of allowed genders.</param>
        public GenderValidator(char[] validGenders)
        {
            this.validGenders = validGenders;
        }

        /// <summary>
        /// Validates sex property of the record.
        /// </summary>
        /// <param name="parameters">Parameters to validate.</param>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (Array.FindIndex(this.validGenders, s => s.Equals(char.ToUpperInvariant(parameters.Sex))) < 0)
            {
                throw new ArgumentException($"Sex can be only: {string.Join(' ', this.validGenders)}", parameters.Sex.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
