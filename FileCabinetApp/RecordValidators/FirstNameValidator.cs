using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for FirstName property validation.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int maxLength;
        private readonly int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxLength">Maximum Length of firstname of the records.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.maxLength = maxLength;
            this.minLength = minLength;
        }

        /// <summary>
        /// Validates Firstname property.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate.</param>
        /// <exception cref="ArgumentNullException">source parameters is null. FirstName property is null, empty of whitespace.</exception>
        /// <exception cref="ArgumentException">FirtName Length is more than <see cref="maxLength"/> or less than <see cref="minLength"/>.</exception>
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

            if (parameters.FirstName.Length < this.minLength || parameters.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {this.maxLength} or less than {this.minLength}", parameters.FirstName);
            }
        }
    }
}
