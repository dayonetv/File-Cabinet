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
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handles 'remove' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private static void Remove(string parameters)
        {
            bool parseResult = int.TryParse(parameters.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int id);

            if (parseResult)
            {
                bool removingResult = Program.FileCabinetService.Remove(id);

                Console.WriteLine(removingResult ? $"Record #{id} is removed." : $"Record #{id} doesn't exists.");
                return;
            }

            Console.WriteLine($"Invalid format for id: {parameters}");
        }
    }
}
