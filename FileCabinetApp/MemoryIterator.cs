using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Iterator for memory service.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records collection to be iterated.</param>
        public MemoryIterator(IReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records?.ToList();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            if (this.records == null || this.records.Count == 0)
            {
                return null;
            }

            FileCabinetRecord record = this.records.First();
            this.records.Remove(record);

            return record;
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.records != null && this.records.Any();
        }
    }
}
