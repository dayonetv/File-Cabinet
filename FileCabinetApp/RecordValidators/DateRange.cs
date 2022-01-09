using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Date range properties for validation-rules file.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// Gets minimal Date.
        /// </summary>
        /// <value></value>
        public DateTime From { get; init; }

        /// <summary>
        /// Gets maximum Date.
        /// </summary>
        /// <value></value>
        public DateTime To { get; init; }
    }
}
