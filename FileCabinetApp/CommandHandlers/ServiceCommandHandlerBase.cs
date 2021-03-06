using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for command handlers that use service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">Current service to be used.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets or sets current service.
        /// </summary>
        /// <value><see cref="IFileCabinetService"/> Service for using.</value>
        protected IFileCabinetService Service { get; set; }
    }
}
