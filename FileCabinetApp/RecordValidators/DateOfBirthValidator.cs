using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for dateOfBirth property validation.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private const string DateFormat = "yyyy-MMM-dd";

        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">DateTime start range.</param>
        /// <param name="to">DateTime end range.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validate DateTime property of the records.
        /// </summary>
        /// <param name="parameters">parameters to validate.</param>
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.DateOfBirth > this.to || parameters.DateOfBirth < this.from)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {this.from.ToString(DateFormat, CultureInfo.InvariantCulture)} or more than {this.to.ToString(DateFormat, CultureInfo.InvariantCulture)}", parameters.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture));
            }
        }
    }
}
