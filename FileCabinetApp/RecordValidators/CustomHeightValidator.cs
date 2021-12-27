using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class CustomHeightValidator : IRecordValidator
    {
        private const short MaxHeight = 220;
        private const short MinHeight = 140;
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public void ValidateParameters(CreateEditParameters parameters)
        {
            if (parameters.Height < MinHeight || parameters.Height > MaxHeight)
            {
                throw new ArgumentException($"Height can not be less than {MinHeight} or more than {MaxHeight}", parameters.Height.ToString(Culture));
            }
        }
     }
}
