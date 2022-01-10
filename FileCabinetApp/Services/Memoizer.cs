using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Memoizer for storing results of <see cref="FileCabinetMemoryService.FindRecords(Dictionary{PropertyInfo, object}, OperationType)"/> method of <see cref="FileCabinetMemoryService"/>, in order to avoid recalculating results.
    /// </summary>
    public class Memoizer
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> argumentRecordsPairs = new (StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Dictionary of properties-values and operation type. </param>
        /// <param name="findedRecords">List of records finded by the key.</param>
        /// <returns>true if the memoizer contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue((Dictionary<PropertyInfo, object>, OperationType) key, out List<FileCabinetRecord> findedRecords)
        {
            string keyToFind = BuildKey(key.Item1, key.Item2);

            return this.argumentRecordsPairs.TryGetValue(keyToFind, out findedRecords);
        }

        /// <summary>
        /// Adds the key and value to the memoizer.
        /// </summary>
        /// <param name="key">Dictionary of properties-values and operation type.</param>
        /// <param name="recordsToAdd">List of records to add by the key.</param>
        public void Add((Dictionary<PropertyInfo, object>, OperationType) key, List<FileCabinetRecord> recordsToAdd)
        {
            string keyToAdd = BuildKey(key.Item1, key.Item2);

            this.argumentRecordsPairs.Add(keyToAdd, recordsToAdd);
        }

        /// <summary>
        /// Removes all keys and values from memorizer.
        /// </summary>
        public void Clear()
        {
            this.argumentRecordsPairs.Clear();
        }

        private static string BuildKey(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation)
        {
            var sortedPropertiesWithValues = propertiesWithValues.OrderBy((key) => key.Key.Name);

            StringBuilder keyBuilder = new ();

            foreach (var propertyNameValue in sortedPropertiesWithValues)
            {
                keyBuilder.Append($"{propertyNameValue.Key.Name}={propertyNameValue.Value} {operation} ");
            }

            return keyBuilder.ToString();
        }
    }
}
