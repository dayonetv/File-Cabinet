namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Represents a range that has maximum and minimum <see cref="int"/> values.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// Gets maximum range value.
        /// </summary>
        /// <value></value>
        public int Max { get; init; }

        /// <summary>
        /// Gets minimum range value.
        /// </summary>
        /// <value></value>
        public int Min { get; init; }
    }
}
