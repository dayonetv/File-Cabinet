using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for building validators.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Validate firstname property of the record.
        /// </summary>
        /// <param name="minLength">Minimal Length of firstname of the records.</param>
        /// <param name="maxLength">Maximum Length of firstname of the records.</param>
        public void ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
        }

        /// <summary>
        /// Validate lastname property of the record.
        /// </summary>
        /// <param name="minLength">Minimal Length of lastname of the records.</param>
        /// <param name="maxLength">Maximum Length of lastname of the records.</param>
        public void ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
        }

        /// <summary>
        /// Validate dateofbirth property of the record.
        /// </summary>
        /// <param name="from">DateTime start range.</param>
        /// <param name="to">DateTime end range.</param>
        public void ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
        }

        /// <summary>
        /// Validate height property of the record.
        /// </summary>
        /// <param name="minHeight">Minimal height.</param>
        /// <param name="maxHeight">Maximum height.</param>
        public void ValidateHeight(int minHeight, int maxHeight)
        {
            this.validators.Add(new HeigthValidator(minHeight, maxHeight));
        }

        /// <summary>
        /// Validate salary property of the record.
        /// </summary>
        /// <param name="minSalary">Minimal salary value.</param>
        /// <param name="maxSalary">Maximal salary value.</param>
        public void ValidateSalary(decimal minSalary, decimal maxSalary)
        {
            this.validators.Add(new SalaryValidator(minSalary, maxSalary));
        }

        /// <summary>
        /// Validate sex property of the record.
        /// </summary>
        /// <param name="validGenders">Valid gender chars.</param>
        public void ValidateGender(char[] validGenders)
        {
            this.validators.Add(new GenderValidator(validGenders));
        }

        /// <summary>
        /// Creates new IRecordValidator.
        /// </summary>
        /// <returns>Validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
