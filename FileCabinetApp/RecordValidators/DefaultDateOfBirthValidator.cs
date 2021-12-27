using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultDateOfBirthValidator : IRecordValidator
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly DateTime MinDateOfBirth = new (1950, 1, 1);
        private const string DateFormat = "yyyy-MMM-dd";

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters.DateOfBirth > DateTime.Now || parameters.DateOfBirth < MinDateOfBirth)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString(DateFormat, Culture)} or more than {DateTime.Now}", parameters.DateOfBirth.ToString(DateFormat, Culture));
            }
        }
    }
}
