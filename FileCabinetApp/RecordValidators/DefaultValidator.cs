using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents default validator.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const short MaxHeight = 220;
        private const short MinHeight = 120;
        private const decimal MaxSalary = 1_000_000;
        private const decimal MinSalary = 0;
        private static readonly char[] ValidGenders = { 'M', 'F' };
        private static readonly DateTime MinDateOfBirth = new (1950, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
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
            return "default validation";
        }
    }
}
