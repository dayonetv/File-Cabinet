using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service for stroring records with the ability to add, edit and find some of them using file system.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordByteSize = 275;
        private const int NameByteSize = 120;

        private static readonly Encoding CurrentEncoding = Encoding.Default;

        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Stream to binary file.</param>
        /// <param name="validator">Current validator to be used. </param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <inheritdoc/>
        public int CreateRecord(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.validator.ValidateParameters(parameters);

            FileCabinetRecord record = new ()
            {
                Id = ((int)this.fileStream.Length / RecordByteSize) + 1,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                Height = parameters.Height,
                Salary = parameters.Salary,
                Sex = parameters.Sex,
            };

            this.WriteRecordToFile(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.fileStream.Seek(default, SeekOrigin.Begin);

            long recordToEditBeginPosition = this.FindRecordById(id);

            if (recordToEditBeginPosition < 0)
            {
                throw new ArgumentException("record is not found", nameof(id));
            }

            FileCabinetRecord updatedRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                Height = parameters.Height,
                Salary = parameters.Salary,
                Sex = parameters.Sex,
            };

            this.fileStream.Seek(recordToEditBeginPosition, SeekOrigin.Begin);

            this.WriteRecordToFile(updatedRecord);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Seek(default, SeekOrigin.Begin);

            List<FileCabinetRecord> readedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.fileStream.Length / RecordByteSize; i++)
            {
                readedRecords.Add(this.ReadOneRecord());
            }

            return readedRecords.AsReadOnly();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return (int)this.fileStream.Length / RecordByteSize;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapShot()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Filesystem service";
        }

        private void WriteRecordToFile(FileCabinetRecord record)
        {
            using (BinaryWriter binWriter = new (this.fileStream, CurrentEncoding, true))
            {
                var nameChars = new char[NameByteSize];

                binWriter.Write(record.Id);

                Array.Copy(record.FirstName.ToCharArray(), nameChars, record.FirstName.Length);
                binWriter.Write(nameChars);

                Array.Copy(record.LastName.ToCharArray(), nameChars, record.LastName.Length);
                binWriter.Write(nameChars);

                binWriter.Write(record.DateOfBirth.Year);
                binWriter.Write(record.DateOfBirth.Month);
                binWriter.Write(record.DateOfBirth.Day);

                binWriter.Write(record.Height);

                binWriter.Write(record.Salary);

                binWriter.Write(record.Sex);
            }
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

        private long FindRecordById(int id)
        {
            using (BinaryReader idsReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                for (int i = 0; i < this.GetStat(); i++)
                {
                    int readedId = idsReader.ReadInt32();

                    if (readedId == id)
                    {
                        return this.fileStream.Position - sizeof(int);
                    }

                    this.fileStream.Seek(RecordByteSize - sizeof(int), SeekOrigin.Current);
                }
            }

            return -1;
        }
    }
}
