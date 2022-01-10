using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service main interface. Provides creation, editing, finding, removing, insertion, restoring, purging records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates new record.
        /// </summary>
        /// <param name="parameters">Record parameters object. </param>
        /// <returns>The Id of created record.</returns>
        /// <exception cref="ArgumentNullException">records parameters object is null.</exception>
        public int CreateRecord(RecordParameters parameters);

        /// <summary>
        /// Gets the information about the amount of current records.
        /// </summary>
        /// <returns>The number of total and deleted records.</returns>
        public (int total, int deleted) GetStat();

        /// <summary>
        /// Edits existing record by it's Id.
        /// </summary>
        /// <param name="id">Record's Id to edit. </param>
        /// <param name="parameters">Record parameters object. </param>
        /// <exception cref="ArgumentNullException">records parameters object is null.</exception>
        public void EditRecord(int id, RecordParameters parameters);

        /// <summary>
        /// Makes snapshot of all current records.
        /// </summary>
        /// <returns>snapshot of current records. </returns>
        public FileCabinetServiceSnapshot MakeSnapShot();

        /// <summary>
        /// Restores records from the file.
        /// </summary>
        /// <param name="snapshot">Snapshot for restoring. </param>
        /// <returns>Information about restored records. </returns>
        /// <exception cref="ArgumentNullException">snapshot is null.</exception>
        public string Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Defragmentates records file for Filesystem Service.
        /// </summary>
        /// <returns>Amount of purged records. </returns>
        public int Purge();

        /// <summary>
        /// Inserts record to the collection.
        /// </summary>
        /// <param name="recordToInsert">Record to insert.</param>
        /// <exception cref="ArgumentNullException">record to insert is null.</exception>
        public void Insert(FileCabinetRecord recordToInsert);

        /// <summary>
        /// Removes record by its property and value of this property.
        /// </summary>
        /// <param name="recordProperty">Property of removing record.</param>
        /// <param name="propertyValue">Value of property.</param>
        /// <returns>List of deleted records identificators.</returns>
        /// <exception cref="ArgumentNullException">record property or property value is null.</exception>
        public List<int> Delete(PropertyInfo recordProperty, object propertyValue);

        /// <summary>
        /// Finds records by propeties and their values.
        /// </summary>
        /// <param name="propertiesWithValues">Dictionary of property and value pairs for finding records.</param>
        /// <param name="operation"><see cref="OperationType"/> for searching properties.</param>
        /// <returns>Collection of finded records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation);
    }
}
