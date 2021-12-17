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
        private const int MaxSalary = int.MaxValue;
        private const short MaxHeight = 220;
        private const short MinHeight = 140;

        private static readonly char[] Genders = { 'M', 'F' };
        private static readonly char[] InvalidNameSymbols = { '!', '@', '#', '$', '%', '^', '&', '*', '.', ',', ':', '~' };
        private static readonly DateTime MinDateOfBirth = new (1940, 1, 1);
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrEmpty(parameters.FirstName) || string.IsNullOrWhiteSpace(parameters.FirstName))
            {
                throw new ArgumentNullException(parameters.FirstName);
            }

            if (string.IsNullOrEmpty(parameters.LastName) || string.IsNullOrWhiteSpace(parameters.LastName))
            {
                throw new ArgumentNullException(parameters.LastName);
            }

            if (parameters.FirstName.Length < MinNameLength || parameters.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }

            if (parameters.LastName.Length < MinNameLength || parameters.LastName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Last Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }

            if (parameters.FirstName.IndexOfAny(InvalidNameSymbols) >= 0)
            {
                throw new ArgumentException($"First Name consits some invalid symbols: {string.Join(' ', InvalidNameSymbols)} -", parameters.FirstName);
            }

            if (parameters.LastName.IndexOfAny(InvalidNameSymbols) >= 0)
            {
                throw new ArgumentException($"Last Name consits some invalid symbols: {string.Join(' ', InvalidNameSymbols)} -", parameters.LastName);
            }

            if (parameters.DateOfBirth > DateTime.Now || parameters.DateOfBirth < MinDateOfBirth)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString("yyyy-MMM-dd", Culture)} or more than {DateTime.Now}", parameters.DateOfBirth.ToString("yyyy-MMM-dd", Culture));
            }

            if (parameters.Salary < 0 || parameters.Salary > MaxSalary)
            {
                throw new ArgumentException($"Salary can not be more than {MaxSalary} or less than 0", parameters.Salary.ToString(Culture));
            }

            if (Array.FindIndex(Genders, s => s.Equals(char.ToUpperInvariant(parameters.Sex))) < 0)
            {
                throw new ArgumentException($"Sex can be only Male or Female", parameters.Sex.ToString(Culture));
            }

            if (parameters.Height < MinHeight || parameters.Height > MaxHeight)
            {
                throw new ArgumentException($"Height can not be less than {MinHeight} or more than {MaxHeight}", parameters.Height.ToString(Culture));
            }
        }
    }
}
