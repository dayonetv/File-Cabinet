using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultFirstNameValidator : IRecordValidator
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;

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
        }
    }
}
