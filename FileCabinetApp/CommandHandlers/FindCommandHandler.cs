using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for find command and find paramaters.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private const string DateFormat = "d";
        private const int AmountOfFindByParams = 2;

        private readonly IRecordPrinter printer;

        private readonly Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] findByFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service. </param>
        /// <param name="printer">Printer for showing records info.</param>
        public FindCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.findByFunctions = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
            {
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", this.FindByFirstName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", this.FindByLastName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", this.FindByDateOfBirth),
            };

            this.printer = printer;
        }

        /// <summary>
        /// Handles 'find' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        public override void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }

        private void Find(string parameters)
        {
            var inputParams = parameters.Trim().Split(' ', AmountOfFindByParams);

            if (inputParams.Length != AmountOfFindByParams)
            {
                Console.WriteLine($"'find' command requires at least {AmountOfFindByParams} parameters. ");
                return;
            }

            string findBy = inputParams[0].Trim();
            string toFind = inputParams[^1].Trim();

            var findByFunc = (from func in this.findByFunctions where func.Item1.Equals(findBy, StringComparison.InvariantCultureIgnoreCase) select func.Item2).FirstOrDefault();

            if (findByFunc == null)
            {
                Console.WriteLine($"Unknown '{findBy}' property for 'find' command. ");
                return;
            }

            ReadOnlyCollection<FileCabinetRecord> findedRecords = findByFunc.Invoke(toFind);

            if (findedRecords != null)
            {
                this.printer.Print(findedRecords);
            }
            else
            {
                Console.WriteLine($"There is no records with {findBy}: '{toFind}'. ");
            }
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateToFind)
        {
            bool parseResult = DateTime.TryParseExact(dateToFind.Trim('"'), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBithToFind);
            return parseResult ? this.Service.FindByDateOfBith(dateOfBithToFind) : null;
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.Service.FindByFirstName(firstName.Trim('"'));
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.Service.FindByLastName(lastName.Trim('"'));
        }
    }
}
