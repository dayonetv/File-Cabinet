using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents default validator.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const short MaxHeight = 220;
        private const short MinHeight = 120;
        private const decimal MaxSalary = 1_000_000;
        private const decimal MinSalary = 0;
        private static readonly char[] ValidGenders = { 'M', 'F' };
        private static readonly DateTime MinDateOfBirth = new (1950, 1, 1);

        /// <inheritdoc/>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            new FirstNameValidator(MinNameLength, MaxNameLength).ValidateParameters(parameters);
            new LastNameValidator(MinNameLength, MaxNameLength).ValidateParameters(parameters);
            new DateOfBirthValidator(MinDateOfBirth, DateTime.Now).ValidateParameters(parameters);
            new HeigthValidator(MinHeight, MaxHeight).ValidateParameters(parameters);
            new SalaryValidator(MinSalary, MaxSalary).ValidateParameters(parameters);
            new GenderValidator(ValidGenders).ValidateParameters(parameters);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "default validation";
        }
    }
}
