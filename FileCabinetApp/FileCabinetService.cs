using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const int MaxSalary = 1_000_000;
        private const short MaxHeight = 220;
        private const short MinHeight = 120;

        private static readonly char[] Genders = { 'M', 'F' };
        private static readonly DateTime MinDateOfBirth = new DateTime(1950, 1, 1);
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char sex)
        {
            ValidateParams(firstName, lastName, dateOfBirth, height, salary, sex);

            FileCabinetRecord record = new ()
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Salary = salary,
                Sex = sex,
            };

            this.list.Add(record);

            this.AddToDictionaries(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return new List<FileCabinetRecord>(this.list).ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char sex)
        {
            FileCabinetRecord recordToEdit = this.list.Find(rec => rec.Id == id);

            if (recordToEdit == null)
            {
                throw new ArgumentException("record is not found", nameof(id));
            }
            else
            {
                ValidateParams(firstName, lastName, dateOfBirth, height, salary, sex);

                this.firstNameDictionary[recordToEdit.FirstName].Remove(recordToEdit);
                this.lastNameDictionary[recordToEdit.LastName].Remove(recordToEdit);
                this.dateOfBirthDictionary[recordToEdit.DateOfBirth].Remove(recordToEdit);

                recordToEdit.FirstName = firstName;
                recordToEdit.LastName = lastName;
                recordToEdit.DateOfBirth = dateOfBirth;
                recordToEdit.Height = height;
                recordToEdit.Salary = salary;
                recordToEdit.Sex = sex;

                this.AddToDictionaries(recordToEdit);
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return this.firstNameDictionary.GetValueOrDefault(firstName)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return this.lastNameDictionary.GetValueOrDefault(lastName)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByDateOfBith(DateTime dateOfBirth)
        {
            return this.dateOfBirthDictionary.GetValueOrDefault(dateOfBirth)?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        private static void ValidateParams(string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char sex)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (firstName.Length < MinNameLength || firstName.Length > MaxNameLength)
            {
                throw new ArgumentException($"First Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", nameof(firstName));
            }

            if (lastName.Length < MinNameLength || lastName.Length > MaxNameLength)
            {
                throw new ArgumentException($"Last Name Lenght is more than {MaxNameLength} or less than {MinNameLength}", nameof(firstName));
            }

            if (dateOfBirth > DateTime.Now || dateOfBirth < MinDateOfBirth)
            {
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString("yyyy-MMM-dd", Culture)} or more than {DateTime.Now}", nameof(dateOfBirth));
            }

            if (salary < 0 || salary > MaxSalary)
            {
                throw new ArgumentException($"Salary can not be more than {MaxSalary} or less than 0", nameof(salary));
            }

            if (Array.FindIndex(Genders, s => s.Equals(char.ToUpperInvariant(sex))) < 0)
            {
                throw new ArgumentException($"Sex can be only Male or Female", nameof(sex));
            }

            if (height < MinHeight || height > MaxHeight)
            {
                throw new ArgumentException($"Height can not be less than {MinHeight} or more than {MaxHeight}", nameof(height));
            }
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
    }
}
