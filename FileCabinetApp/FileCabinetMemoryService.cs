using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents service for stroring records with the ability to add, edit and find some of them.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class with specific validator.
        /// </summary>
        /// <param name="validator">Validator to be used. </param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates new record and adds its to the list.
        /// </summary>
        /// <param name="parameters">Parameter object. </param>
        /// <returns>The Id of created record.</returns>
        public int CreateRecord(CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.validator.ValidateParameters(parameters);

            FileCabinetRecord record = new ()
            {
                Id = this.list.Count + 1,
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                DateOfBirth = parameters.DateOfBirth,
                Height = parameters.Height,
                Salary = parameters.Salary,
                Sex = parameters.Sex,
            };

            this.list.Add(record);

            this.AddToDictionaries(record);

            return record.Id;
        }

        /// <summary>
        /// Gets a copy of array of current created records.
        /// </summary>
        /// <returns>Array of current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.list.AsReadOnly();
        }

        /// <summary>
        /// Gets the information about the amount of current existing records.
        /// </summary>
        /// <returns>The number of existing records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edits existing record by it's Id.
        /// </summary>
        /// <param name="id">Record Id to edit by. </param>
        /// <param name="parameters">Parameter object. </param>
        public void EditRecord(int id, CreateEditParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            FileCabinetRecord recordToEdit = this.list.Find(rec => rec.Id == id);

            if (recordToEdit == null)
            {
                throw new ArgumentException("record is not found", nameof(id));
            }
            else
            {
                this.validator.ValidateParameters(parameters);

                this.RemoveFromDictionaries(recordToEdit);

                recordToEdit.FirstName = parameters.FirstName;
                recordToEdit.LastName = parameters.LastName;
                recordToEdit.DateOfBirth = parameters.DateOfBirth;
                recordToEdit.Height = parameters.Height;
                recordToEdit.Salary = parameters.Salary;
                recordToEdit.Sex = parameters.Sex;

                this.AddToDictionaries(recordToEdit);
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapShot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords());
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
                    CreateEditParameters parameters = RecordToParameters(record);

                    this.validator.ValidateParameters(parameters);
                }
                catch (ArgumentException ex)
                {
                    recordsInfo.Append($"ID {record.Id}: {ex.Message}\n");
                    invalidRecords.Add(record);
                }
            }

            invalidRecords.ForEach((invalidRec) => recordsToAdd.Remove(invalidRec));

            if (recordsToAdd.Count == 0)
            {
                return recordsInfo.ToString() + "No records imported";
            }

            int recordsToAddStartId = recordsToAdd[0].Id;
            int recirdsToAddEndID = recordsToAdd[^1].Id;

            List<FileCabinetRecord> recordsToEdit = this.list.FindAll((rec) => rec.Id >= recordsToAddStartId && rec.Id <= recirdsToAddEndID);

            if (recordsToEdit.Count != 0)
            {
                foreach (var record in recordsToEdit)
                {
                    this.RemoveFromDictionaries(record);

                    this.list.Remove(record);
                }
            }

            this.list.AddRange(recordsToAdd);
            recordsToAdd.ForEach(this.AddToDictionaries);

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

            List<FileCabinetRecord> findedRecords
                = this.list.FindAll((record) => recordProperty.GetValue(record).ToString().Equals(propertyValue.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (findedRecords.Count != 0)
            {
                foreach (var record in findedRecords)
                {
                    this.RemoveFromDictionaries(record);
                    this.list.Remove(record);
                }
            }

            return findedRecords.Select((rec) => rec.Id).ToList();
        }

        /// <inheritdoc/>
        public int Purge()
        {
            return default;
        }

        /// <summary>
        /// Searches records by First Name in curent records using special 'firstNameDictionary' dictionary.
        /// </summary>
        /// <param name="firstName">First Name to search by.</param>
        /// <returns>Iterator for finded records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new MemoryFindedRecords(this.firstNameDictionary.GetValueOrDefault(firstName));
        }

        /// <summary>
        /// Searches a records by Last Name in curent records using special 'lastNameDictionary' dictionary.
        /// </summary>
        /// <param name="lastName">Last Name to search by.</param>
        /// <returns>Iterator for finded records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return new MemoryFindedRecords(this.lastNameDictionary.GetValueOrDefault(lastName));
        }

        /// <summary>
        /// Searches a records by Date of birth in curent records using special 'dateOfBirthDictionary' dictionary.
        /// </summary>
        /// <param name="dateOfBirth">Date of Birth to search by, in format "yyyy-MMM-dd".</param>
        /// <returns>Iterator for finded records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth)
        {
            return new MemoryFindedRecords(this.dateOfBirthDictionary.GetValueOrDefault(dateOfBirth));
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

            FileCabinetRecord findedRecord = this.list.Find((rec) => rec.Id == recordToInsert.Id);

            if (findedRecord != null)
            {
                this.RemoveFromDictionaries(findedRecord);
                this.list.Remove(findedRecord);
            }

            this.list.Add(recordToInsert);
            this.AddToDictionaries(recordToInsert);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Memory service";
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

        private void AddToDictionaries(FileCabinetRecord recordToAdd)
        {
            if (!this.firstNameDictionary.ContainsKey(recordToAdd.FirstName))
            {
                this.firstNameDictionary.Add(recordToAdd.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(recordToAdd.LastName))
            {
                this.lastNameDictionary.Add(recordToAdd.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(recordToAdd.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(recordToAdd.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[recordToAdd.FirstName].Add(recordToAdd);
            this.lastNameDictionary[recordToAdd.LastName].Add(recordToAdd);
            this.dateOfBirthDictionary[recordToAdd.DateOfBirth].Add(recordToAdd);
        }

        private void RemoveEmptyDictionaryKeys(FileCabinetRecord removedRecord)
        {
            if (this.firstNameDictionary[removedRecord.FirstName].Count == 0)
            {
                this.firstNameDictionary.Remove(removedRecord.FirstName);
            }

            if (this.lastNameDictionary[removedRecord.LastName].Count == 0)
            {
                this.lastNameDictionary.Remove(removedRecord.LastName);
            }

            if (this.dateOfBirthDictionary[removedRecord.DateOfBirth].Count == 0)
            {
                this.dateOfBirthDictionary.Remove(removedRecord.DateOfBirth);
            }
        }

        private void RemoveFromDictionaries(FileCabinetRecord recordToRemove)
        {
            this.firstNameDictionary[recordToRemove.FirstName].Remove(recordToRemove);
            this.lastNameDictionary[recordToRemove.LastName].Remove(recordToRemove);
            this.dateOfBirthDictionary[recordToRemove.DateOfBirth].Remove(recordToRemove);

            this.RemoveEmptyDictionaryKeys(recordToRemove);
        }
    }
}
