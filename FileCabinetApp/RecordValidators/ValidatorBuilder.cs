using System;
using System.Collections.Generic;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Class for building validators.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Builds <see cref="FirstNameValidator"/>.
        /// </summary>
        /// <param name="minLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxLength">Maximum Length of firstname of the records.</param>
        public void ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
        }

        /// <summary>
        /// Builds <see cref="LastNameValidator"/>.
        /// </summary>
        /// <param name="minLength">Minimal Length of lastname of the records.</param>
        /// <param name="maxLength">Maximum Length of lastname of the records.</param>
        public void ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
        }

        /// <summary>
        /// Builds <see cref="DateOfBirthValidator"/>.
        /// </summary>
        /// <param name="from">DateTime start range.</param>
        /// <param name="to">DateTime end range.</param>
        public void ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
        }

        /// <summary>
        /// Builds <see cref="HeightValidator"/>.
        /// </summary>
        /// <param name="minHeight">Minimal height.</param>
        /// <param name="maxHeight">Maximum height.</param>
        public void ValidateHeight(int minHeight, int maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
        }

        /// <summary>
        /// Builds <see cref="SalaryValidator"/>.
        /// </summary>
        /// <param name="minSalary">Minimal salary value.</param>
        /// <param name="maxSalary">Maximal salary value.</param>
        public void ValidateSalary(decimal minSalary, decimal maxSalary)
        {
            this.validators.Add(new SalaryValidator(minSalary, maxSalary));
        }

        /// <summary>
        /// Builds <see cref="GenderValidator"/>.
        /// </summary>
        /// <param name="validGenders">Valid gender chars.</param>
        public void ValidateGender(char[] validGenders)
        {
            this.validators.Add(new GenderValidator(validGenders));
        }

        /// <summary>
        /// Creates new <see cref="CompositeValidator"/> composed of builded validators.
        /// </summary>
        /// <returns>Created validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
