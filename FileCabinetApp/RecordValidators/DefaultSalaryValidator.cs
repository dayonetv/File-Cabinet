using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultSalaryValidator : IRecordValidator
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private const int MaxSalary = 1_000_000;

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters.Salary < 0 || parameters.Salary > MaxSalary)
            {
                throw new ArgumentException($"Salary can not be more than {MaxSalary} or less than 0", parameters.Salary.ToString(Culture));
            }
        }
    }
}
