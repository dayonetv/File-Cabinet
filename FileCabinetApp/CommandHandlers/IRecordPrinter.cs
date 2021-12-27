using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Printer for showing records information.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints records information.
        /// </summary>
        /// <param name="records">Records to be printed.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
