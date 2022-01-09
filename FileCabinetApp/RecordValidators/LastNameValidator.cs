using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for firstname property validation.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int maxLength;
        private readonly int minLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxLength">Maximum Length of firstname of the records.</param>
        public LastNameValidator(int minLength, int maxLength)
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

            if (string.IsNullOrEmpty(parameters.LastName) || string.IsNullOrWhiteSpace(parameters.LastName))
            {
                throw new ArgumentNullException(parameters.LastName);
            }

            if (parameters.LastName.Length < this.minLength || parameters.LastName.Length > this.maxLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {this.maxLength} or less than {this.minLength}", parameters.FirstName);
            }
        }
    }
}
