using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        private const string DateFormat = "yyyy-MMM-dd";
        private static readonly DateTime MinDateOfBirth = new (1940, 1, 1);
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters.DateOfBirth > DateTime.Now || parameters.DateOfBirth < MinDateOfBirth)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString(DateFormat, Culture)} or more than {DateTime.Now}", parameters.DateOfBirth.ToString(DateFormat, Culture));
            }
        }
    }
}
