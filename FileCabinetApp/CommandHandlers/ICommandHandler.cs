using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Interface for creating Command Handlers.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next command hadler that will handle request.
        /// </summary>
        /// <param name="commandHandler">Next command handler.</param>
        /// <returns>Setted next command handler. </returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="request">Request to be handled. </param>
        public void Handle(AppCommandRequest request);
    }
}
