namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Represents command and parameters that should be handled.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets string representaion of the command.
        /// </summary>
        /// <value>String command.</value>
        public string Command { get; init; }

        /// <summary>
        /// Gets string representation of parameters.
        /// </summary>
        /// <value>String command.</value>
        public string Parameters { get; init; }
    }
}
