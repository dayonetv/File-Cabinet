using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Collection of finded records by memory service.
    /// </summary>
    public class MemoryFindedRecords : IEnumerable<FileCabinetRecord>
    {
        private readonly IReadOnlyCollection<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryFindedRecords"/> class.
        /// </summary>
        /// <param name="records">Collection of finded records.</param>
        public MemoryFindedRecords(IReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new MemoryIterator(this.records);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MemoryIterator(this.records);
        }
    }
}
