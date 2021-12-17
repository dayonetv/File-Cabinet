using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service for stroring records with the ability to add, edit records using default validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Creates default record validator.
        /// </summary>
        /// <returns>Default record validator (DefaultValidator).</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
