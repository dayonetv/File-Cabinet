using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Extension methods for creating Default and Custom validators.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        private const string DefaultSection = "default";
        private const string CustomSection = "custom";

        private static readonly FileInfo RulesFile = new FileInfo("validation-rules.json");
        private static readonly IConfiguration ValidationConfiguration = new ConfigurationBuilder().AddJsonFile(RulesFile.FullName, true, true).Build();

        /// <summary>
        /// Creates Default validator.
        /// </summary>
        /// <param name="builder">builder instance.</param>
        /// <returns>Default Validator.</returns>
        /// <exception cref="ArgumentNullException">builder is null.</exception>
        public static IRecordValidator Default(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ProcessValidationRules(builder, ValidationConfiguration.GetSection(DefaultSection));

            return builder.Create();
        }

        /// <summary>
        /// Creates Custom validator.
        /// </summary>
        /// <param name="builder">builder instance.</param>
        /// <returns>Custom Validator.</returns>
        /// <exception cref="ArgumentNullException">builder is null.</exception>
        public static IRecordValidator Custom(this ValidatorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ProcessValidationRules(builder, ValidationConfiguration.GetSection(CustomSection));

            return builder.Create();
        }

        private static void ProcessValidationRules(ValidatorBuilder builder, IConfigurationSection validationModeSection)
        {
            ValidationRule currentRule = validationModeSection.Get<ValidationRule>();

            builder.ValidateFirstName(currentRule.FirstName.Min, currentRule.FirstName.Max);

            builder.ValidateLastName(currentRule.LastName.Min, currentRule.LastName.Max);

            builder.ValidateDateOfBirth(currentRule.DateOfBirth.From, currentRule.DateOfBirth.To);

            builder.ValidateHeight(currentRule.Height.Min, currentRule.Height.Max);

            builder.ValidateSalary(currentRule.Salary.Min, currentRule.Salary.Max);

            builder.ValidateGender(currentRule.Genders.ToArray());
        }
    }
}
