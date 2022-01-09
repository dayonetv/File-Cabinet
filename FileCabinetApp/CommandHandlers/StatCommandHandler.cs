using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for stat command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "stat";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'stat' command or moves request to the next handler.
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
                this.Stat();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Stat()
        {
            (int total, int deleted) recordsCount = this.Service.GetStat();

            Console.WriteLine($"Total: {recordsCount.total} record(s).\nDeleted: {recordsCount.deleted} record(s).");
        }
    }
}
