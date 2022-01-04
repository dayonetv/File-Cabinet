using System;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Extension methods for creating Default and Custom.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        private const int DefaultMaxNameLength = 60;
        private const int DefaultMinNameLength = 2;
        private const int CustomMaxNameLength = 60;
        private const int CustomMinNameLength = 2;
        private const decimal DefaultMaxSalary = 1_000_000;
        private const decimal CustomMaxSalary = int.MaxValue;
        private const short DefaultMaxHeight = 220;
        private const short DefaultMinHeight = 120;
        private const short CustomMaxHeight = 220;
        private const short CustomMinHeight = 140;

        private static readonly char[] CustomValidGenders = { 'M', 'F' };
        private static readonly char[] DefaultValidGenders = { 'M', 'F' };
        private static readonly DateTime DefaultMinDateOfBirth = new (1950, 1, 1);
        private static readonly DateTime CustomMinDateOfBirth = new (1940, 1, 1);

        /// <summary>
        /// Creates Default Validator.
        /// </summary>
        /// <param name="builder">builder instance.</param>
        /// <returns>Default Validator.</returns>
        public static IRecordValidator Default(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ValidateFirstName(DefaultMinNameLength, DefaultMaxNameLength);
            builder.ValidateLastName(DefaultMinNameLength, DefaultMaxNameLength);
            builder.ValidateDateOfBirth(DefaultMinDateOfBirth, DateTime.Now);
            builder.ValidateHeight(DefaultMinHeight, DefaultMaxHeight);
            builder.ValidateSalary(0, DefaultMaxSalary);
            builder.ValidateGender(DefaultValidGenders);
            return builder.Create();
        }

        /// <summary>
        /// Creates Custom validator.
        /// </summary>
        /// <param name="builder">builder instance.</param>
        /// <returns>Custom Validator.</returns>
        public static IRecordValidator Custom(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ValidateFirstName(CustomMinNameLength, CustomMaxNameLength);
            builder.ValidateLastName(CustomMinNameLength, CustomMaxNameLength);
            builder.ValidateDateOfBirth(CustomMinDateOfBirth, DateTime.Now);
            builder.ValidateHeight(CustomMinHeight, CustomMaxHeight);
            builder.ValidateSalary(0, CustomMaxSalary);
            builder.ValidateGender(CustomValidGenders);
            return builder.Create();
        }
    }
}
