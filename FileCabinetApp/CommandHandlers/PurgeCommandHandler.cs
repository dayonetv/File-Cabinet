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
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handles 'purge' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private void Purge()
        {
            if (this.service is FileCabinetMemoryService)
            {
                Console.WriteLine($"{this.service} has nothing to purge. ");
                return;
            }

            int allrecordsAmount = this.service.GetStat();
            int purgedAmount = this.service.Purge();

            Console.WriteLine($"Data file processing is completed: {purgedAmount} of {allrecordsAmount} records were purged.");
        }
    }
}
