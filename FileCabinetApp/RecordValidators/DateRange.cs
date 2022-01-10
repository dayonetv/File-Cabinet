using System;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    ///  Represents a date range that has from and to <see cref="DateTime"/> values.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// Gets minimum DateTime.
        /// </summary>
        /// <value>Minimum DateTime.</value>
        public DateTime From { get; init; }

        /// <summary>
        /// Gets maximum DateTime.
        /// </summary>
        /// <value>Maxumum DateTime.</value>
        public DateTime To { get; init; }
    }
}
