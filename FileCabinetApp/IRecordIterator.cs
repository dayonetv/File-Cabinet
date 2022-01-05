using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Iterator for records collection.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets next record.
        /// </summary>
        /// <returns>Next record.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Determines whether the collection contains more records to get.
        /// </summary>
        /// <returns>True if the collection has more records to return, oterwise - false.</returns>
        public bool HasMore();
    }
}
