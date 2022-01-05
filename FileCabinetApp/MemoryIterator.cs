using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Enumerator for memory service finded records collection.
    /// </summary>
    public sealed class MemoryIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> records;
        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Records collection to be iterated.</param>
        public MemoryIterator(IReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records?.ToList();
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current => this.records[this.position];

        /// <inheritdoc/>
        object IEnumerator.Current => this.records[this.position];

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            this.position++;
            return this.records != null && this.records.Count != this.position;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.position = default;
        }
    }
}
