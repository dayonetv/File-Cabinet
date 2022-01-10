using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Support handling of <see cref="AppCommandRequest"/>.
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
        /// Handles request or moves request to the next handler.
        /// </summary>
        /// <param name="request">Request to be handled. </param>
        /// <exception cref="ArgumentNullException">request is null.</exception>
        public void Handle(AppCommandRequest request);
    }
}
