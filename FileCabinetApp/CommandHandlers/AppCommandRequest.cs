using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents command and parameters that should be handled.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets string representaion of the command.
        /// </summary>
        /// <value></value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets string representation of parameters.
        /// </summary>
        /// <value></value>
        public string Parameters { get; set; }
    }
}
