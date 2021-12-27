using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handles 'stat' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static void Stat()
        {
            var totalRecordsCount = Program.FileCabinetService.GetStat();
            var deletedRecordsCount = totalRecordsCount - Program.FileCabinetService.GetRecords().Count;

            Console.WriteLine($"Total: {totalRecordsCount} record(s).\nDeleted: {deletedRecordsCount} record(s).");
        }
    }
}
