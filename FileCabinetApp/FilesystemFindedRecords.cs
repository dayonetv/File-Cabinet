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
        private const int NameByteSize = 120;
        private static readonly Encoding CurrentEncoding = Encoding.Default;

        private readonly IReadOnlyCollection<long> recordsOffsets;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemFindedRecords"/> class.
        /// </summary>
        /// <param name="offsets">Finded Records offsets in the file.</param>
        /// <param name="fileStream">Stream to storing file.</param>
        public FilesystemFindedRecords(IReadOnlyCollection<long> offsets, FileStream fileStream)
        {
            this.recordsOffsets = offsets ?? new List<long>();
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var offset in this.recordsOffsets)
            {
                yield return this.ReadOneRecord(offset);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
