using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for remove command and remove paramaters.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "remove";

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'remove' command or moves request to the next handler.
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
                this.Remove(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Remove(string parameters)
        {
            bool parseResult = int.TryParse(parameters.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int id);

            if (parseResult)
            {
                bool removingResult = this.Service.Remove(id);

                Console.WriteLine(removingResult ? $"Record #{id} is removed." : $"Record #{id} doesn't exists.");
                return;
            }

            Console.WriteLine($"Invalid format for id: {parameters}");
        }
    }
}
