using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for edit command and edit paramaters.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "edit";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        public EditCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles 'edit' command or moves request to the next handler.
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
                this.Edit(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Edit(string parameters)
        {
            bool isValid = true;
            do
            {
                try
                {
                    bool parseResult = int.TryParse(parameters, NumberStyles.Any, CultureInfo.InvariantCulture, out int id);

                    FileCabinetRecord recordToEdit = parseResult ? this.Service.GetRecords()?.FirstOrDefault(rec => rec.Id == id) : null;

                    if (recordToEdit == null)
                    {
                        Console.WriteLine($"#{parameters} record is not found.");
                    }
                    else
                    {
                        CreateEditParameters updatedParams = EnterInfo();

                        this.Service.EditRecord(id, updatedParams);

                        Console.WriteLine($"Record #{id} is updated.");

                        isValid = true;
                    }
                }
                catch (ArgumentException ex)
                {
                    isValid = false;
                    Console.WriteLine($"\n{ex.Message}. Please try again.");
                }
            }
            while (!isValid);
        }
    }
}
