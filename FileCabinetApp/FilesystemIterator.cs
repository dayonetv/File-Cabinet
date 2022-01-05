using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Iterator for filesystem service.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private const int NameByteSize = 120;
        private static readonly Encoding CurrentEncoding = Encoding.Default;

        private readonly List<long> recordsOffsets;
        private readonly FileStream fileStream;

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
        public FileCabinetRecord GetNext()
        {
            if (this.recordsOffsets == null || this.recordsOffsets.Count == 0)
            {
                return null;
            }

            long recordPosition = this.recordsOffsets.First();

            this.fileStream.Seek(recordPosition, SeekOrigin.Begin);

            this.recordsOffsets.Remove(recordPosition);

            return this.ReadOneRecord();
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.recordsOffsets != null && this.recordsOffsets.Any();
        }

        private FileCabinetRecord ReadOneRecord()
        {
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
