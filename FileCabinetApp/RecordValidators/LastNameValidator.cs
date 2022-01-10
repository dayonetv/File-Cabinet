using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for FirstName property validation.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int maxLastNameLength;
        private readonly int minLastNameLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLastNameLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxLastNameLength">Maximum Length of firstname of the records.</param>
        public LastNameValidator(int minLastNameLength, int maxLastNameLength)
        {
            this.maxLastNameLength = maxLastNameLength;
            this.minLastNameLength = minLastNameLength;
        }

        /// <summary>
        /// Validates LastName property of the record.
        /// </summary>
        /// <param name="parameters">Parameters to validate.</param>
        /// <exception cref="ArgumentNullException">Source parameters is null. LastName property is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentException">LastName Length is more than <see cref="maxLastNameLength"/> or less than <see cref="minLastNameLength"/>.</exception>
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

            if (parameters.LastName.Length < this.minLastNameLength || parameters.LastName.Length > this.maxLastNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {this.maxLastNameLength} or less than {this.minLastNameLength}", parameters.FirstName);
            }
        }
    }
}
