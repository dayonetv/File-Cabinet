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
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "purge";

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'purge' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Purge();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Purge()
        {
            if (this.Service is FileCabinetMemoryService)
            {
                Console.WriteLine($"{this.Service} has nothing to purge. ");
                return;
            }

            int totalRecordsAmount = this.Service.GetStat().total;
            int purgedAmount = this.Service.Purge();

            Console.WriteLine($"Data file processing is completed: {purgedAmount} of {totalRecordsAmount} records were purged.");
        }
    }
}
