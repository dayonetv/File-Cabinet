namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents application command and parameters to be handled.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets string representaion of the command.
        /// </summary>
        /// <value><see cref="string"/> command.</value>
        public string Command { get; init; }

        /// <summary>
        /// Gets parameters string representation.
        /// </summary>
        /// <value><see cref="string"/> parameters.</value>
        public string Parameters { get; init; }
    }
}
