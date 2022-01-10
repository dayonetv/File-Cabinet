using System;
using System.Globalization;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for Height property validation.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private readonly int minHeight;
        private readonly int maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Minimal height.</param>
        /// <param name="maxHeight">Maximum height.</param>
        public HeightValidator(int minHeight, int maxHeight)
        {
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// Validates Height property of the record.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate. </param>
        /// <exception cref="ArgumentNullException">source parameters is null.</exception>
        /// <exception cref="ArgumentException">Height is more than <see cref="maxHeight"/> or less than <see cref="minHeight"/>.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Height < this.minHeight || parameters.Height > this.maxHeight)
            {
                throw new ArgumentException($"Height can not be less than {this.minHeight} or more than {this.maxHeight}", parameters.Height.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
