using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> stopRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="stopRunning">Delegate to stop running Programm.</param>
        public ExitCommandHandler(Action<bool> stopRunning)
        {
            this.stopRunning = stopRunning;
        }

        /// <summary>
        /// Handles 'exit' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.stopRunning?.Invoke(false);
        }
    }
}
