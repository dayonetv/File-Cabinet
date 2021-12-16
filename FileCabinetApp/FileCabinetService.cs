using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents abstract service for stroring records with the ability to add, edit and find some of them.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

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

            this.ValidateParams(parameters);

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
        public FileCabinetRecord[] GetRecords()
        {
            return new List<FileCabinetRecord>(this.list).ToArray();
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
                this.ValidateParams(parameters);

                this.firstNameDictionary[recordToEdit.FirstName].Remove(recordToEdit);
                this.lastNameDictionary[recordToEdit.LastName].Remove(recordToEdit);
                this.dateOfBirthDictionary[recordToEdit.DateOfBirth].Remove(recordToEdit);

                recordToEdit.FirstName = parameters.FirstName;
                recordToEdit.LastName = parameters.LastName;
                recordToEdit.DateOfBirth = parameters.DateOfBirth;
                recordToEdit.Height = parameters.Height;
                recordToEdit.Salary = parameters.Salary;
                recordToEdit.Sex = parameters.Sex;

                this.AddToDictionaries(recordToEdit);
            }
        }

        /// <summary>
        /// Searches records by First Name in curent records using special 'firstNameDictionary' dictionary.
        /// </summary>
        /// <param name="firstName">First Name to search by.</param>
        /// <returns>Array of finded records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return this.firstNameDictionary.GetValueOrDefault(firstName)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches a records by Last Name in curent records using special 'lastNameDictionary' dictionary.
        /// </summary>
        /// <param name="lastName">Last Name to search by.</param>
        /// <returns>Array of finded records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return this.lastNameDictionary.GetValueOrDefault(lastName)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches a records by Date of birth in curent records using special 'dateOfBirthDictionary' dictionary.
        /// </summary>
        /// <param name="dateOfBirth">Date of Birth to search by, in format "yyyy-MMM-dd".</param>
        /// <returns>Array of finded records.</returns>
        public FileCabinetRecord[] FindByDateOfBith(DateTime dateOfBirth)
        {
            return this.dateOfBirthDictionary.GetValueOrDefault(dateOfBirth)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Checks parameters correctness.
        /// </summary>
        /// <param name="parameters">Parameter object to validate. </param>
        protected abstract void ValidateParams(CreateEditParameters parameters);

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
    }
}
