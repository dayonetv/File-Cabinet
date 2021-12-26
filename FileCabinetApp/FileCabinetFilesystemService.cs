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
        private const int FirstNameOffset = 4;
        private const int LastNameOffset = 124;
        private const int DateOfBirthOffset = 244;

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

            this.fileStream.Seek(RecordByteSize * this.GetStat(), SeekOrigin.Begin);

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
            List<FileCabinetRecord> findedRecords = new List<FileCabinetRecord>();

            this.fileStream.Seek(DateOfBirthOffset, SeekOrigin.Begin);

            int amountOfRecords = this.GetStat();
            using (BinaryReader dateReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                for (int i = 0; i < amountOfRecords; i++)
                {
                    DateTime readedDate = new DateTime(dateReader.ReadInt32(), dateReader.ReadInt32(), dateReader.ReadInt32());

                    if (readedDate == dateOfBirth)
                    {
                        this.fileStream.Seek(i * RecordByteSize, SeekOrigin.Begin);
                        findedRecords.Add(this.ReadOneRecord());
                    }
                    else
                    {
                        this.fileStream.Seek((i + 1) * RecordByteSize, SeekOrigin.Begin);
                    }

                    this.fileStream.Seek(DateOfBirthOffset, SeekOrigin.Current);
                }
            }

            return findedRecords.Count != default ? findedRecords.AsReadOnly() : null;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> findedRecords = new List<FileCabinetRecord>();

            this.fileStream.Seek(FirstNameOffset, SeekOrigin.Begin);

            int amountOfRecords = this.GetStat();

            using (BinaryReader firstNameReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                for (int i = 0; i < amountOfRecords; i++)
                {
                    string readedFirstName = CurrentEncoding.GetString(firstNameReader.ReadBytes(NameByteSize)).Trim('\0');

                    if (readedFirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.fileStream.Seek(i * RecordByteSize, SeekOrigin.Begin);
                        findedRecords.Add(this.ReadOneRecord());
                    }
                    else
                    {
                        this.fileStream.Seek((i + 1) * RecordByteSize, SeekOrigin.Begin);
                    }

                    this.fileStream.Seek(FirstNameOffset, SeekOrigin.Current);
                }
            }

            return findedRecords.Count != default ? findedRecords.AsReadOnly() : null;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> findedRecords = new List<FileCabinetRecord>();

            this.fileStream.Seek(LastNameOffset, SeekOrigin.Begin);

            int amountOfRecords = this.GetStat();

            using (BinaryReader lastNameReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                for (int i = 0; i < amountOfRecords; i++)
                {
                    string readedLastName = CurrentEncoding.GetString(lastNameReader.ReadBytes(NameByteSize)).Trim('\0');

                    if (readedLastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.fileStream.Seek(i * RecordByteSize, SeekOrigin.Begin);
                        findedRecords.Add(this.ReadOneRecord());
                    }
                    else
                    {
                        this.fileStream.Seek((i + 1) * RecordByteSize, SeekOrigin.Begin);
                    }

                    this.fileStream.Seek(LastNameOffset, SeekOrigin.Current);
                }
            }

            return findedRecords.Count != default ? findedRecords.AsReadOnly() : null;
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
            return new FileCabinetServiceSnapshot(this.GetRecords());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Filesystem service";
        }

        /// <inheritdoc/>
        public string Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            StringBuilder recordsInfo = new StringBuilder();

            List<FileCabinetRecord> recordsToAdd = new List<FileCabinetRecord>(snapshot.Records);

            if (recordsToAdd.Count == 0)
            {
                return "No records imported";
            }

            List<FileCabinetRecord> invalidRecords = new List<FileCabinetRecord>();

            foreach (var record in recordsToAdd)
            {
                try
                {
                    CreateEditParameters parameters = new CreateEditParameters()
                    {
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        DateOfBirth = record.DateOfBirth,
                        Height = record.Height,
                        Salary = record.Salary,
                        Sex = record.Sex,
                    };

                    this.validator.ValidateParameters(parameters);
                }
                catch (ArgumentException ex)
                {
                    recordsInfo.Append($"ID {record.Id}: {ex.Message}\n");
                    invalidRecords.Add(record);
                }
            }

            invalidRecords.ForEach((invalidRec) => recordsToAdd.Remove(invalidRec));

            foreach (var record in recordsToAdd)
            {
                this.fileStream.Seek(default, SeekOrigin.Begin);

                long findedRecordPosition = this.FindRecordById(record.Id);

                if (findedRecordPosition >= 0)
                {
                    this.fileStream.Seek(findedRecordPosition, SeekOrigin.Begin);
                }
                else
                {
                    this.fileStream.Seek(this.fileStream.Length, SeekOrigin.Begin);
                }

                this.WriteRecordToFile(record);
            }

            return recordsInfo.ToString() + $"{recordsToAdd.Count} records were imported ";
        }

        /// <inheritdoc/>
        public bool Remove(int id)
        {
            throw new NotImplementedException();
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
                int amountOfRecords = this.GetStat();

                for (int i = 0; i < amountOfRecords; i++)
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
