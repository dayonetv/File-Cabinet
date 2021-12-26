using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service main interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates new record and adds its to the list.
        /// </summary>
        /// <param name="parameters">Parameter object. </param>
        /// <returns>The Id of created record.</returns>
        public int CreateRecord(CreateEditParameters parameters);

        /// <summary>
        /// Gets a copy of array of current created records.
        /// </summary>
        /// <returns>Array of current records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Gets the information about the amount of current existing records.
        /// </summary>
        /// <returns>The number of existing records.</returns>
        public int GetStat();

        /// <summary>
        /// Edits existing record by it's Id.
        /// </summary>
        /// <param name="id">Record Id to edit by. </param>
        /// <param name="parameters">Parameter object. </param>
        public void EditRecord(int id, CreateEditParameters parameters);

        /// <summary>
        /// Searches records by First Name in curent records.
        /// </summary>
        /// <param name="firstName">First Name to search by.</param>
        /// <returns>Array of finded records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches a records by Last Name in curent records.
        /// </summary>
        /// <param name="lastName">Last Name to search by.</param>
        /// <returns>Array of finded records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches a records by Date of birth in curent records using.
        /// </summary>
        /// <param name="dateOfBirth">Date of Birth to search by.</param>
        /// <returns>Array of finded records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBith(DateTime dateOfBirth);

        /// <summary>
        /// Makes snapshot of all current records.
        /// </summary>
        /// <returns>snapshot of current records. </returns>
        public FileCabinetServiceSnapshot MakeSnapShot();

        /// <summary>
        /// Restore records from file.
        /// </summary>
        /// <param name="snapshot">Snapshot for restoring. </param>
        /// <returns>Information about restored records. </returns>
        public string Restore(FileCabinetServiceSnapshot snapshot);
    }
}
