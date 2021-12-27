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
        private const int MaxSalary = 1_000_000;
        private const short MaxHeight = 220;
        private const short MinHeight = 120;
        private const string DateFormat = "yyyy-MMM-dd";

        private static readonly char[] Genders = { 'M', 'F' };
        private static readonly DateTime MinDateOfBirth = new (1950, 1, 1);
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            ValidateFirstName(parameters);
            ValidateLastName(parameters);
            ValidateDateOfBirth(parameters);
            ValidateHeight(parameters);
            ValidateSalary(parameters);
            ValidateGender(parameters);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "default validation";
        }

        private static void ValidateFirstName(CreateEditParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.FirstName) || string.IsNullOrWhiteSpace(parameters.FirstName))
            {
                throw new ArgumentNullException(parameters.FirstName);
            }

            if (parameters.FirstName.Length < MinNameLength || parameters.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }
        }

        private static void ValidateLastName(CreateEditParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.LastName) || string.IsNullOrWhiteSpace(parameters.LastName))
            {
                throw new ArgumentNullException(parameters.LastName);
            }

            if (parameters.LastName.Length < MinNameLength || parameters.LastName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Last Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }
        }

        private static void ValidateDateOfBirth(CreateEditParameters parameters)
        {
            if (parameters.DateOfBirth > DateTime.Now || parameters.DateOfBirth < MinDateOfBirth)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString(DateFormat, Culture)} or more than {DateTime.Now}", parameters.DateOfBirth.ToString(DateFormat, Culture));
            }
        }

        private static void ValidateHeight(CreateEditParameters parameters)
        {
            if (parameters.Height < MinHeight || parameters.Height > MaxHeight)
            {
                throw new ArgumentException($"Height can not be less than {MinHeight} or more than {MaxHeight}", parameters.Height.ToString(Culture));
            }
        }

        private static void ValidateSalary(CreateEditParameters parameters)
        {
            if (parameters.Salary < 0 || parameters.Salary > MaxSalary)
            {
                throw new ArgumentException($"Salary can not be more than {MaxSalary} or less than 0", parameters.Salary.ToString(Culture));
            }
        }

        private static void ValidateGender(CreateEditParameters parameters)
        {
            if (Array.FindIndex(Genders, s => s.Equals(char.ToUpperInvariant(parameters.Sex))) < 0)
            {
                throw new ArgumentException($"Sex can be only Male or Female", parameters.Sex.ToString(Culture));
            }
        }
    }
}
