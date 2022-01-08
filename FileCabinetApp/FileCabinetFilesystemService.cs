using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service for stroring records with the ability to add, edit and find some of them using file system.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordByteSize = 276;
        private const int NameByteSize = 120;
        private const int IsDeletedOffset = 275;

        private const bool IsDeletedDefaultValue = false;

        private static readonly Encoding CurrentEncoding = Encoding.Default;

        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;

        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

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

            long recordBeginPosition = this.fileStream.Length;

            this.fileStream.Seek(recordBeginPosition, SeekOrigin.Begin);

            this.WriteRecordToFile(record);

            this.AddToDictionaries(record, recordBeginPosition);

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

            this.validator.ValidateParameters(parameters);

            this.RemoveFromDictionaries(recordToEditBeginPosition);

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

            this.AddToDictionaries(updatedRecord, recordToEditBeginPosition);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth)
        {
            return new FilesystemFindedRecords(this.dateOfBirthDictionary.GetValueOrDefault(dateOfBirth), this.fileStream);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new FilesystemFindedRecords(this.firstNameDictionary.GetValueOrDefault(firstName), this.fileStream);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return new FilesystemFindedRecords(this.lastNameDictionary.GetValueOrDefault(lastName), this.fileStream);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation)
        {
            List<FileCabinetRecord> allRecords = this.GetRecords().ToList();

            if (propertiesWithValues == null || propertiesWithValues.Count == 0)
            {
                return allRecords.AsReadOnly();
            }

            List<FileCabinetRecord> findedRecords = new List<FileCabinetRecord>();

            switch (operation)
            {
                case OperationType.None: case OperationType.And: findedRecords.AddRange(allRecords); break;
            }

            foreach (var propertyValue in propertiesWithValues)
            {
                List<FileCabinetRecord> findedRecordsByOneProperty = new List<FileCabinetRecord>();

                switch (operation)
                {
                    case OperationType.None: case OperationType.And:
                        findedRecords = findedRecords.FindAll((record) => propertyValue.Key.GetValue(record).ToString().Equals(propertyValue.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
                        continue;
                    case OperationType.Or:
                        findedRecordsByOneProperty.AddRange(allRecords.FindAll((record) => propertyValue.Key.GetValue(record).ToString().Equals(propertyValue.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)));
                        break;
                }

                findedRecords = findedRecords.Union(findedRecordsByOneProperty).ToList();
            }

            return findedRecords.AsReadOnly();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Seek(default, SeekOrigin.Begin);

            List<FileCabinetRecord> readedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.fileStream.Length / RecordByteSize; i++)
            {
                var record = this.ReadOneRecord();

                if (record != null)
                {
                    readedRecords.Add(record);
                }
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
                    this.RemoveFromDictionaries(findedRecordPosition);

                    this.fileStream.Seek(findedRecordPosition, SeekOrigin.Begin);
                }
                else
                {
                    this.fileStream.Seek(this.fileStream.Length, SeekOrigin.Begin);
                }

                this.AddToDictionaries(record, this.fileStream.Position);
                this.WriteRecordToFile(record);
            }

            return recordsInfo.ToString() + $"{recordsToAdd.Count} records were imported ";
        }

        /// <inheritdoc/>
        public List<int> Delete(PropertyInfo recordProperty, object propertyValue)
        {
            if (recordProperty == null)
            {
                throw new ArgumentNullException(nameof(recordProperty));
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            List<FileCabinetRecord> findedRecords =
                this.GetRecords().ToList().FindAll((record) => recordProperty.GetValue(record).ToString().Equals(propertyValue.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (findedRecords.Count != 0)
            {
                foreach (var record in findedRecords)
                {
                    this.Remove(record.Id);
                }
            }

            return findedRecords.Select((rec) => rec.Id).ToList();
        }

        /// <inheritdoc/>
        public int Purge()
        {
            int amountOfAllRecords = (int)this.fileStream.Length / RecordByteSize;

            List<FileCabinetRecord> notDeletedRecords = new List<FileCabinetRecord>(this.GetRecords());

            if (notDeletedRecords.Count != amountOfAllRecords)
            {
                this.firstNameDictionary.Clear();
                this.lastNameDictionary.Clear();
                this.dateOfBirthDictionary.Clear();

                this.fileStream.SetLength(notDeletedRecords.Count * RecordByteSize);
                this.fileStream.Seek(default, SeekOrigin.Begin);

                foreach (var record in notDeletedRecords)
                {
                    this.AddToDictionaries(record, this.fileStream.Position);
                    this.WriteRecordToFile(record);
                }
            }

            return amountOfAllRecords - notDeletedRecords.Count;
        }

        /// <inheritdoc/>
        public void Insert(FileCabinetRecord recordToInsert)
        {
            if (recordToInsert == null)
            {
                throw new ArgumentNullException(nameof(recordToInsert));
            }

            if (recordToInsert.Id <= 0)
            {
                throw new ArgumentException("Id should be more than 0.", nameof(recordToInsert));
            }

            this.validator.ValidateParameters(RecordToParameters(recordToInsert));

            long findedRecordPosition = this.FindRecordById(recordToInsert.Id);

            if (findedRecordPosition >= 0)
            {
                this.RemoveFromDictionaries(findedRecordPosition);

                this.fileStream.Seek(findedRecordPosition, SeekOrigin.Begin);
                this.WriteRecordToFile(recordToInsert);

                this.AddToDictionaries(recordToInsert, findedRecordPosition);
            }
            else
            {
                long recordToInsertPosition = this.fileStream.Length;

                this.fileStream.Seek(recordToInsertPosition, SeekOrigin.Begin);
                this.WriteRecordToFile(recordToInsert);

                this.AddToDictionaries(recordToInsert, recordToInsertPosition);
            }
        }

        private static CreateEditParameters RecordToParameters(FileCabinetRecord record)
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

            return parameters;
        }

        private void Remove(int id)
        {
            long recordToRemovePosition = this.FindRecordById(id);

            if (recordToRemovePosition < 0)
            {
                return;
            }

            this.RemoveFromDictionaries(recordToRemovePosition);

            using (BinaryWriter binWriter = new BinaryWriter(this.fileStream, CurrentEncoding, true))
            {
                this.fileStream.Seek(recordToRemovePosition + IsDeletedOffset, SeekOrigin.Begin);
                binWriter.Write(!IsDeletedDefaultValue);
            }
        }

        private void WriteRecordToFile(FileCabinetRecord record)
        {
            using (BinaryWriter binWriter = new (this.fileStream, CurrentEncoding, true))
            {
                binWriter.Write(record.Id);

                var nameChars = new char[NameByteSize];
                Array.Copy(record.FirstName.ToCharArray(), nameChars, record.FirstName.Length);
                binWriter.Write(nameChars);

                nameChars = new char[NameByteSize];
                Array.Copy(record.LastName.ToCharArray(), nameChars, record.LastName.Length);
                binWriter.Write(nameChars);

                binWriter.Write(record.DateOfBirth.Year);
                binWriter.Write(record.DateOfBirth.Month);
                binWriter.Write(record.DateOfBirth.Day);

                binWriter.Write(record.Height);

                binWriter.Write(record.Salary);

                binWriter.Write(record.Sex);

                binWriter.Write(IsDeletedDefaultValue);
            }
        }

        private FileCabinetRecord ReadOneRecord()
        {
            FileCabinetRecord readedRecord = new FileCabinetRecord();
            bool isDeleted = false;

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

                isDeleted = binReader.ReadBoolean();
            }

            return isDeleted ? null : readedRecord;
        }

        private long FindRecordById(int id)
        {
            using (BinaryReader idsReader = new BinaryReader(this.fileStream, CurrentEncoding, true))
            {
                int amountOfRecords = this.GetStat();

                for (int i = 0; i < amountOfRecords; i++)
                {
                    this.fileStream.Seek((i * RecordByteSize) + IsDeletedOffset, SeekOrigin.Begin);
                    bool isDeleted = idsReader.ReadBoolean();
                    this.fileStream.Seek(i * RecordByteSize, SeekOrigin.Begin);

                    int readedId = idsReader.ReadInt32();

                    if (readedId == id && !isDeleted)
                    {
                        return this.fileStream.Position - sizeof(int);
                    }

                    this.fileStream.Seek(RecordByteSize - sizeof(int), SeekOrigin.Current);
                }
            }

            return -1;
        }

        private void AddToDictionaries(FileCabinetRecord recordToAdd, long recordOffset)
        {
            if (!this.firstNameDictionary.ContainsKey(recordToAdd.FirstName))
            {
                this.firstNameDictionary.Add(recordToAdd.FirstName, new List<long>());
            }

            if (!this.lastNameDictionary.ContainsKey(recordToAdd.LastName))
            {
                this.lastNameDictionary.Add(recordToAdd.LastName, new List<long>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(recordToAdd.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(recordToAdd.DateOfBirth, new List<long>());
            }

            this.firstNameDictionary[recordToAdd.FirstName].Add(recordOffset);
            this.lastNameDictionary[recordToAdd.LastName].Add(recordOffset);
            this.dateOfBirthDictionary[recordToAdd.DateOfBirth].Add(recordOffset);
        }

        private void RemoveFromDictionaries(long recordOffset, FileCabinetRecord recordToRemove = null)
        {
            this.fileStream.Seek(recordOffset, SeekOrigin.Begin);

            recordToRemove ??= this.ReadOneRecord();

            if (recordToRemove == null)
            {
                return;
            }

            this.firstNameDictionary[recordToRemove.FirstName].Remove(recordOffset);

            if (this.firstNameDictionary[recordToRemove.FirstName].Count == 0)
            {
                this.firstNameDictionary.Remove(recordToRemove.FirstName);
            }

            this.lastNameDictionary[recordToRemove.LastName].Remove(recordOffset);

            if (this.lastNameDictionary[recordToRemove.LastName].Count == 0)
            {
                this.lastNameDictionary.Remove(recordToRemove.LastName);
            }

            this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Remove(recordOffset);

            if (this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Count == 0)
            {
                this.dateOfBirthDictionary.Remove(recordToRemove.DateOfBirth);
            }
        }
    }
}
