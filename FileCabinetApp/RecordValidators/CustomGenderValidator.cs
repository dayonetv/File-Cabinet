using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomGenderValidator : IRecordValidator
    {
        private static readonly char[] Genders = { 'M', 'F' };
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (Array.FindIndex(Genders, s => s.Equals(char.ToUpperInvariant(parameters.Sex))) < 0)
            {
                throw new ArgumentException($"Sex can be only Male or Female", parameters.Sex.ToString(Culture));
            }
        }
     }
}
