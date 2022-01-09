using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents methods that shoud be implemented by inheritors.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Checks parameters correctness.
        /// </summary>
        /// <param name="parameters">Parameter object to validate. </param>
        public void ValidateParameters(RecordParameters parameters);
    }
}
