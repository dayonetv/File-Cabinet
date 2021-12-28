using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "create";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'create' command or moves request to the next handler.
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
                this.Create();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Create()
        {
            bool isValid;
            do
            {
                try
                {
                    CreateEditParameters creationParams = EnterInfo();

                    Console.WriteLine($"Record #{this.Service.CreateRecord(creationParams)} is created.");

                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again. ");
                }
            }
            while (!isValid);
        }
    }
}
