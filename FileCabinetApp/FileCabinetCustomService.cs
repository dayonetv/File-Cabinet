using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service for stroring records with the ability to add, edit records using custom validation.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Creates custom record validator.
        /// </summary>
        /// <returns>Custom record validator (CustomValidaotr).</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
