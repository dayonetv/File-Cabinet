using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomFirstNameValidator : IRecordValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private static readonly char[] InvalidNameSymbols = { '!', '@', '#', '$', '%', '^', '&', '*', '.', ',', ':', '~' };

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.FirstName) || string.IsNullOrWhiteSpace(parameters.FirstName))
            {
                throw new ArgumentNullException(parameters.FirstName);
            }

            if (parameters.FirstName.Length < MinNameLength || parameters.FirstName.Length > MaxNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }

            if (parameters.FirstName.IndexOfAny(InvalidNameSymbols) >= 0)
            {
                throw new ArgumentException($"First Name consits some invalid symbols: {string.Join(' ', InvalidNameSymbols)} -", parameters.FirstName);
            }
        }
    }
}
