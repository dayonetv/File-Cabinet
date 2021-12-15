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
        private static readonly CultureInfo CurrentCulture = CultureInfo.InvariantCulture;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

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

                recordToEdit.FirstName = firstName;
                recordToEdit.LastName = lastName;
                recordToEdit.DateOfBirth = dateOfBirth;
                recordToEdit.Height = height;
                recordToEdit.Salary = salary;
                recordToEdit.Sex = sex;
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return (from rec in this.list where rec.FirstName.ToUpperInvariant() == firstName select rec).ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return (from rec in this.list where rec.LastName.ToUpperInvariant() == lastName select rec).ToArray();
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
                throw new ArgumentException($"Date Of Birth can not be less than {MinDateOfBirth.ToString("yyyy-MMM-dd", CurrentCulture)} or more than now", nameof(dateOfBirth));
            }

            if (salary < 0 || salary > MaxSalary)
            {
                throw new ArgumentException("Salary can not be more than 1_000_000 or less than 0", nameof(salary));
            }

            if (char.ToLower(sex, CurrentCulture) != 'm' && char.ToLower(sex, CurrentCulture) != 'f')
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
