using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for height property validation.
    /// </summary>
    public class HeigthValidator : IRecordValidator
    {
        private readonly int minHeight;
        private readonly int maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeigthValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Minimal height.</param>
        /// <param name="maxHeight">Maximum height.</param>
        public HeigthValidator(int minHeight, int maxHeight)
        {
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
        }

        /// <summary>
        /// Validates height property of the record.
        /// </summary>
        /// <param name="parameters">Parameters to validate. </param>
        public void ValidateParameters(CreateEditParameters parameters)
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
