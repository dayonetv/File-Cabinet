using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for firstname property validation.
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
        /// Validate Firstname property.
        /// </summary>
        /// <param name="parameters">Parameters to validate.</param>
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
