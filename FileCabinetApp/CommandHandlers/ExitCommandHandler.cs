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
        /// <summary>
        /// Handles 'exit' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }
    }
}
