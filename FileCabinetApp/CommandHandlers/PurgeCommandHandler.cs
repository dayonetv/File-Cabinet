using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for purge command and purge paramaters.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handles 'purge' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static void Purge()
        {
            if (Program.FileCabinetService is FileCabinetMemoryService)
            {
                Console.WriteLine($"{Program.FileCabinetService} has nothing to purge. ");
                return;
            }

            int allrecordsAmount = Program.FileCabinetService.GetStat();
            int purgedAmount = Program.FileCabinetService.Purge();

            Console.WriteLine($"Data file processing is completed: {purgedAmount} of {allrecordsAmount} records were purged.");
        }
    }
}
