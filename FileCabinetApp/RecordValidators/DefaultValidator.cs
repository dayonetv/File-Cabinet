using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents default validator.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var firstNameValidator = new DefaultFirstNameValidator();
            var lastNameValidator = new DefaultLastNameValidator();
            var dateOfBirthValidator = new DefaultDateOfBirthValidator();
            var heightValidator = new DefaultHeightValidator();
            var salaryValidator = new DefaultSalaryValidator();
            var genderValidator = new DefaultGenderValidator();

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
            return "default validation";
        }
    }
}
