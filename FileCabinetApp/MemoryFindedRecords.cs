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
            this.records = records ?? new List<FileCabinetRecord>();
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var record in this.records)
            {
                yield return record;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
