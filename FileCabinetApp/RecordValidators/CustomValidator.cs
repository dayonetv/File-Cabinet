using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents custom record validator.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var firstNameValidator = new CustomFirstNameValidator();
            var lastNameValidator = new CustomLastNameValidator();
            var dateOfBirthValidator = new CustomDateOfBirthValidator();
            var heightValidator = new CustomHeightValidator();
            var salaryValidator = new CustomSalaryValidator();
            var genderValidator = new CustomGenderValidator();

            firstNameValidator.ValidateParameters(parameters);
            lastNameValidator.ValidateParameters(parameters);
            dateOfBirthValidator.ValidateParameters(parameters);
            heightValidator.ValidateParameters(parameters);
            salaryValidator.ValidateParameters(parameters);
            genderValidator.ValidateParameters(parameters);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "custom validation";
        }
    }
}
