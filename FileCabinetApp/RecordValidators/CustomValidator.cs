using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents custom record validator.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const short MaxHeight = 220;
        private const short MinHeight = 140;
        private const int MinSalary = 0;
        private const int MaxSalary = int.MaxValue;
        private static readonly DateTime MinDateOfBirth = new (1940, 1, 1);
        private static readonly char[] ValidGenders = { 'M', 'F' };

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
            return "custom validation";
        }
    }
}
