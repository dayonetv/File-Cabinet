using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using FileCabinetApp.CommandHandlers;

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
        /// Gets the information about the amount of current existing records.
        /// </summary>
        /// <returns>The number of total and deleted records.</returns>
        public (int total, int deleted) GetStat();

        /// <summary>
        /// Edits existing record by it's Id.
        /// </summary>
        /// <param name="id">Record Id to edit by. </param>
        /// <param name="parameters">Parameter object. </param>
        public void EditRecord(int id, CreateEditParameters parameters);

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

        /// <summary>
        /// Defragmentate records file for Filesystem Service.
        /// </summary>
        /// <returns>Amount of purged records. </returns>
        public int Purge();

        /// <summary>
        /// Insert record to the collection.
        /// </summary>
        /// <param name="recordToInsert">Record to insert.</param>
        public void Insert(FileCabinetRecord recordToInsert);

        /// <summary>
        /// Remove record by its property and value of this property.
        /// </summary>
        /// <param name="recordProperty">Property of removing record.</param>
        /// <param name="propertyValue">Value of property.</param>
        /// <returns>List of deleted records identificators.</returns>
        public List<int> Delete(PropertyInfo recordProperty, object propertyValue);

        /// <summary>
        /// Finds records by propety and property-value.
        /// </summary>
        /// <param name="propertiesWithValues">Properties and their value for finding records.</param>
        /// <param name="operation">Operation type for compound searching by several properties.</param>
        /// <returns>Finded records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation);
    }
}
