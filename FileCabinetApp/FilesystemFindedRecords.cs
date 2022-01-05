using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Collection of finded records by filesystem service.
    /// </summary>
    public class FilesystemFindedRecords : IEnumerable<FileCabinetRecord>
    {
        private readonly IReadOnlyCollection<long> recordsOffsets;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemFindedRecords"/> class.
        /// </summary>
        /// <param name="offsets">Records offsets int the file.</param>
        /// <param name="fileStream">Stream to records storing file.</param>
        public FilesystemFindedRecords(IReadOnlyCollection<long> offsets, FileStream fileStream)
        {
            this.recordsOffsets = offsets;
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemIterator(this.recordsOffsets, this.fileStream);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FilesystemIterator(this.recordsOffsets, this.fileStream);
        }
    }
}
