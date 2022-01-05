using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Enumerator for filesystem service finded records collection.
    /// </summary>
    public sealed class FilesystemIterator : IEnumerator<FileCabinetRecord>
    {
        private const int NameByteSize = 120;
        private static readonly Encoding CurrentEncoding = Encoding.Default;

        private readonly List<long> recordsOffsets;
        private readonly FileStream fileStream;

        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="offsets">Records offsets int the file.</param>
        /// <param name="fileStream">Stream to records storing file.</param>
        public FilesystemIterator(IReadOnlyCollection<long> offsets, FileStream fileStream)
        {
            this.recordsOffsets = offsets?.ToList();
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current => this.ReadOneRecord(this.recordsOffsets[this.position]);

        /// <inheritdoc/>
        object IEnumerator.Current => this.ReadOneRecord(this.recordsOffsets[this.position]);

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            this.position++;
            return this.recordsOffsets != null && this.recordsOffsets.Count != this.position;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.position = default;
        }

        private FileCabinetRecord ReadOneRecord(long recordPosition)
        {
            this.fileStream.Seek(recordPosition, SeekOrigin.Begin);

            FileCabinetRecord readedRecord = new FileCabinetRecord();

            using (BinaryReader binReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                readedRecord.Id = binReader.ReadInt32();

                readedRecord.FirstName = CurrentEncoding.GetString(binReader.ReadBytes(NameByteSize)).Trim('\0');
                readedRecord.LastName = CurrentEncoding.GetString(binReader.ReadBytes(NameByteSize)).Trim('\0');

                int yearOfBirth = binReader.ReadInt32();
                int monthOfBirth = binReader.ReadInt32();
                int dayOfBirth = binReader.ReadInt32();
                readedRecord.DateOfBirth = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);

                readedRecord.Height = binReader.ReadInt16();

                readedRecord.Salary = binReader.ReadDecimal();

                readedRecord.Sex = binReader.ReadChar();
            }

            return readedRecord;
        }
    }
}
