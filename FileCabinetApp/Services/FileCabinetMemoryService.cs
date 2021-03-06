using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Represents service for stroring records using List of <see cref="FileCabinetRecord"/>.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        private readonly IRecordValidator validator;

        private readonly Memoizer memoizer = new ();

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
        /// <exception cref="ArgumentNullException">Parameters is null.</exception>
        public int CreateRecord(RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.validator.ValidateParameters(parameters);

            this.memoizer.Clear();

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

        /// <inheritdoc/>
        public (int total, int deleted) GetStat()
        {
            return (this.list.Count, default);
        }

        /// <summary>
        /// Edits existing record by it's Id.
        /// </summary>
        /// <param name="id">Record Id to edit by. </param>
        /// <param name="parameters">Parameter object. </param>
        public void EditRecord(int id, RecordParameters parameters)
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

                this.memoizer.Clear();
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapShot()
        {
            return new FileCabinetServiceSnapshot(this.list.AsReadOnly());
        }

        /// <inheritdoc/>
        public string Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            StringBuilder importedRecordsInfo = new StringBuilder();

            List<FileCabinetRecord> recordsToAdd = new (snapshot.Records);

            if (recordsToAdd.Count == 0)
            {
                return "No records imported";
            }

            List<FileCabinetRecord> invalidRecords = new ();

            foreach (var record in recordsToAdd)
            {
                try
                {
                    RecordParameters parameters = (RecordParameters)record;

                    this.validator.ValidateParameters(parameters);
                }
                catch (ArgumentException ex)
                {
                    importedRecordsInfo.Append($"ID {record.Id}: {ex.Message}\n");
                    invalidRecords.Add(record);
                }
            }

            invalidRecords.ForEach((invalidRec) => recordsToAdd.Remove(invalidRec));

            if (recordsToAdd.Count == 0)
            {
                return importedRecordsInfo.ToString() + "No records imported";
            }

            List<int> recordsToAddIdentificators = (from record in recordsToAdd select record.Id).ToList();

            List<FileCabinetRecord> recordsToEdit = new ();

            foreach (var id in recordsToAddIdentificators)
            {
                recordsToEdit.AddRange(this.list.FindAll((rec) => rec.Id == id));
            }

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

            this.memoizer.Clear();

            return importedRecordsInfo.ToString() + $"{recordsToAdd.Count} records were imported ";
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
                this.memoizer.Clear();

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

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation)
        {
            if (propertiesWithValues == null || propertiesWithValues.Count == 0)
            {
                return this.list.AsReadOnly();
            }

            if (this.memoizer.TryGetValue((propertiesWithValues, operation), out List<FileCabinetRecord> findedRecords))
            {
                return findedRecords.AsReadOnly();
            }

            findedRecords = new List<FileCabinetRecord>();

            switch (operation)
            {
                case OperationType.None: case OperationType.And: findedRecords.AddRange(this.list); break;
                case OperationType.Or: findedRecords.Clear(); break;
                default: return findedRecords.AsReadOnly();
            }

            PropertyInfo firstnameProperty = typeof(FileCabinetRecord).GetProperty(nameof(FileCabinetRecord.FirstName));
            PropertyInfo lastNameProperty = typeof(FileCabinetRecord).GetProperty(nameof(FileCabinetRecord.LastName));
            PropertyInfo dateOfBirthProperty = typeof(FileCabinetRecord).GetProperty(nameof(FileCabinetRecord.DateOfBirth));

            foreach (var propertyValue in propertiesWithValues)
            {
                List<FileCabinetRecord> findedRecordsByOneProperty = new ();

                if (propertyValue.Key.Equals(firstnameProperty))
                {
                    findedRecordsByOneProperty.AddRange(this.FindByFirstName(propertyValue.Value.ToString()));
                }
                else if (propertyValue.Key.Equals(lastNameProperty))
                {
                    findedRecordsByOneProperty.AddRange(this.FindByLastName(propertyValue.Value.ToString()));
                }
                else if (propertyValue.Key.Equals(dateOfBirthProperty))
                {
                    findedRecordsByOneProperty.AddRange(this.FindByDateOfBith((DateTime)propertyValue.Value));
                }
                else
                {
                    switch (operation)
                    {
                        case OperationType.None: case OperationType.And:
                            findedRecords = findedRecords.FindAll((record) => propertyValue.Key.GetValue(record).ToString().Equals(propertyValue.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
                            continue;
                        case OperationType.Or:
                            findedRecordsByOneProperty.AddRange(this.list.FindAll((record) => propertyValue.Key.GetValue(record).ToString().Equals(propertyValue.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)));
                            break;
                    }
                }

                switch (operation)
                {
                    case OperationType.None: case OperationType.And: findedRecords = findedRecords.Intersect(findedRecordsByOneProperty).ToList(); break;
                    case OperationType.Or: findedRecords = findedRecords.Union(findedRecordsByOneProperty).ToList(); break;
                }
            }

            this.memoizer.Add((propertiesWithValues, operation), findedRecords);

            return findedRecords.AsReadOnly();
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

            this.validator.ValidateParameters((RecordParameters)recordToInsert);

            this.memoizer.Clear();

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

        /// <summary>
        /// Searches records by First Name in curent records using special 'firstNameDictionary' dictionary.
        /// </summary>
        /// <param name="firstName">First Name to search by.</param>
        /// <returns>IEnumberable collection of finded records.</returns>
        private IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new MemoryFindedRecords(this.firstNameDictionary.GetValueOrDefault(firstName));
        }

        /// <summary>
        /// Searches a records by Last Name in curent records using special 'lastNameDictionary' dictionary.
        /// </summary>
        /// <param name="lastName">Last Name to search by.</param>
        /// <returns>IEnumberable collection of finded records.</returns>
        private IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return new MemoryFindedRecords(this.lastNameDictionary.GetValueOrDefault(lastName));
        }

        /// <summary>
        /// Searches a records by Date of birth in curent records using special 'dateOfBirthDictionary' dictionary.
        /// </summary>
        /// <param name="dateOfBirth">Date of Birth to search by.</param>
        /// <returns>IEnumberable collection of finded records.</returns>
        private IEnumerable<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth)
        {
            return new MemoryFindedRecords(this.dateOfBirthDictionary.GetValueOrDefault(dateOfBirth));
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
