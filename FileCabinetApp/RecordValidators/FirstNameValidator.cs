using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for FirstName property validation.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int maxFirstNameLength;
        private readonly int minFirstNameLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minFirstNameLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxFirstNameLength">Maximum Length of firstname of the records.</param>
        public FirstNameValidator(int minFirstNameLength, int maxFirstNameLength)
        {
            this.maxFirstNameLength = maxFirstNameLength;
            this.minFirstNameLength = minFirstNameLength;
        }

        /// <summary>
        /// Validates Firstname property.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate.</param>
        /// <exception cref="ArgumentNullException">source parameters is null. FirstName property is null, empty of whitespace.</exception>
        /// <exception cref="ArgumentException">FirtName Length is more than <see cref="maxFirstNameLength"/> or less than <see cref="minFirstNameLength"/>.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrEmpty(parameters.FirstName) || string.IsNullOrWhiteSpace(parameters.FirstName))
            {
                throw new ArgumentNullException(parameters.FirstName);
            }

            if (parameters.FirstName.Length < this.minFirstNameLength || parameters.FirstName.Length > this.maxFirstNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {this.maxFirstNameLength} or less than {this.minFirstNameLength}", parameters.FirstName);
            }
        }
    }
}
