using System;
using System.Globalization;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for Sex property validation.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char[] validGenders;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="validGenders">Array of allowed genders symbols.</param>
        public GenderValidator(char[] validGenders)
        {
            this.validGenders = validGenders;
        }

        /// <summary>
        /// Validates Sex property of the record.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate.</param>
        /// <exception cref="ArgumentNullException">source parameters is null.</exception>
        /// <exception cref="ArgumentException">Sex contains invalid chars.</exception>
        public void ValidateParameters(RecordParameters parameters)
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
