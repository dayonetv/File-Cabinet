using System;
using System.Globalization;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for Salary property validation of the record.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal maxSalary;
        private readonly decimal minSalary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minSalary">Minimal salary value.</param>
        /// <param name="maxSalary">Maximal salary value.</param>
        public SalaryValidator(decimal minSalary, decimal maxSalary)
        {
            this.maxSalary = maxSalary;
            this.minSalary = minSalary;
        }

        /// <summary>
        /// Validates Salary property of the record.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate. </param>
        /// <exception cref="ArgumentNullException">Source parameters is null.</exception>
        /// <exception cref="ArgumentException">Salary is more than <see cref="maxSalary"/> or less than <see cref="minSalary"/>.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Salary < this.minSalary || parameters.Salary > this.maxSalary)
            {
                throw new ArgumentException($"Salary can not be more than {this.maxSalary} or less than {this.minSalary}", parameters.Salary.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
