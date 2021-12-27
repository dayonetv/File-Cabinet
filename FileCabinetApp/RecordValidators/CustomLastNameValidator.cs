using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomLastNameValidator : IRecordValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private static readonly char[] InvalidNameSymbols = { '!', '@', '#', '$', '%', '^', '&', '*', '.', ',', ':', '~' };

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.LastName) || string.IsNullOrWhiteSpace(parameters.LastName))
            {
                throw new ArgumentNullException(parameters.LastName);
            }

            if (parameters.LastName.Length < MinNameLength || parameters.LastName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Last Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", parameters.FirstName);
            }

            if (parameters.LastName.IndexOfAny(InvalidNameSymbols) >= 0)
            {
                throw new ArgumentException($"Last Name consits some invalid symbols: {string.Join(' ', InvalidNameSymbols)} -", parameters.LastName);
            }
        }
    }
}
