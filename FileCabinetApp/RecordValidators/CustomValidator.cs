using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents custom record validator.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const short MaxHeight = 220;
        private const short MinHeight = 140;
        private const int MinSalary = 0;
        private const int MaxSalary = int.MaxValue;
        private static readonly DateTime MinDateOfBirth = new (1940, 1, 1);
        private static readonly char[] ValidGenders = { 'M', 'F' };

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
                  {
                      new FirstNameValidator(MinNameLength, MaxNameLength),
                      new LastNameValidator(MinNameLength, MaxNameLength),
                      new DateOfBirthValidator(MinDateOfBirth, DateTime.Now),
                      new HeigthValidator(MinHeight, MaxHeight),
                      new SalaryValidator(MinSalary, MaxSalary),
                      new GenderValidator(ValidGenders),
                  })
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "custom validation";
        }
    }
}
