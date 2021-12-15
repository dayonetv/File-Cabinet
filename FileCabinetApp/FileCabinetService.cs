using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private const int MaxNameLength = 60;
        private const int MinNameLength = 2;
        private const int MaxSalary = 1_000_000;
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

            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[record.FirstName].Add(record);
            this.lastNameDictionary[record.LastName].Add(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(record);

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

                if (!this.firstNameDictionary.ContainsKey(recordToEdit.FirstName))
                {
                    this.firstNameDictionary.Add(recordToEdit.FirstName, new List<FileCabinetRecord>());
                }

                if (!this.lastNameDictionary.ContainsKey(recordToEdit.LastName))
                {
                    this.lastNameDictionary.Add(recordToEdit.LastName, new List<FileCabinetRecord>());
                }

                this.firstNameDictionary[recordToEdit.FirstName].Add(recordToEdit);
                this.lastNameDictionary[recordToEdit.LastName].Add(recordToEdit);
                this.dateOfBirthDictionary[recordToEdit.DateOfBirth].Add(recordToEdit);
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
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString("yyyy-MMM-dd", Culture)} or more than now", nameof(dateOfBirth));
            }

            if (salary < 0 || salary > MaxSalary)
            {
                throw new ArgumentException("Salary can not be more than 1_000_000 or less than 0", nameof(salary));
            }

            if (char.ToLower(sex, Culture) != 'm' && char.ToLower(sex, Culture) != 'f')
            {
                throw new ArgumentException("Sex can be only Male or Female", nameof(sex));
            }

            if (height < 120 || height > 220)
            {
                throw new ArgumentException("Height can not be less 120 than or more than 220", nameof(height));
            }
        }
    }
}
