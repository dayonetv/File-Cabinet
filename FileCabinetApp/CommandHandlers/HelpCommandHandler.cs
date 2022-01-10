using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for 'help' command and paramaters.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "help";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private const double SimilarityMinimalRatio = 0.4;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics", "The 'stat' command prints statistics" },
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "export", "exports all records to the file", "The 'export' command exports all records to the file." },
            new string[] { "import", "imports records from the file", "The 'import' command imports records from the file." },
            new string[] { "purge", "defragmentates records file for Filesystem Service", "The 'purge' defragmentates records file for Filesystem Service." },
            new string[] { "insert", "inserts record to the service", "The 'insert' command inserts record to the service" },
            new string[] { "delete", "deletes records by values of their properties", "The 'delete' command deletes records by values of their properties" },
            new string[] { "update", "updates records by values of their properties, excepting Id", "The 'update' command updates records by values of their properties, excepting Id" },
            new string[] { "select", "selects records by values of their properties and prints required properties of finded records", "The 'select' command selects records by values of their properties and prints required properties of finded records" },
        };

        /// <summary>
        /// Handles 'help' command or moves request to the next handler.
        /// </summary>
        /// <param name="request">Command and parameters to be handled.</param>
        /// <exception cref="ArgumentNullException">request is null.</exception>
        public override void Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!CheckCommand(request.Command))
            {
                PrintMissedCommandInfo(request.Command);
                return;
            }

            if (request.Command.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintHelp(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static bool CheckCommand(string command)
        {
            int indexOfCommand = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, msg => string.Equals(msg[CommandHelpIndex], command, StringComparison.InvariantCultureIgnoreCase));

            return indexOfCommand >= 0;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");

            DisplaySimilarCommands(command);

            Console.WriteLine();
        }

        private static List<string> FindSimilarCommands(string incorrectCommand)
        {
            List<string> suggestions = new ();

            foreach (var command in HelpMessages)
            {
                if (GetSimilarity(incorrectCommand.ToUpperInvariant(), command[CommandHelpIndex].ToUpperInvariant()) >= SimilarityMinimalRatio)
                {
                    suggestions.Add(command[CommandHelpIndex]);
                }
            }

            return suggestions;
        }

        private static double GetSimilarity(string source, string target)
        {
            int sourceWordLength = source.Length;
            int targetWordLength = target.Length;

            int[][] distance = new int[sourceWordLength + 1][];

            for (int i = 0; i < distance.GetLongLength(default); i++)
            {
                distance[i] = new int[targetWordLength + 1];
            }

            for (int i = 0; i <= sourceWordLength; i++)
            {
                distance[i][0] = i;
            }

            for (int j = 0; j <= targetWordLength; j++)
            {
                distance[0][j] = j;
            }

            for (int i = 1; i <= sourceWordLength; i++)
            {
                for (int j = 1; j <= targetWordLength; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    distance[i][j] = Math.Min(Math.Min(distance[i - 1][j] + 1, distance[i][j - 1] + 1), distance[i - 1][j - 1] + cost);
                }
            }

            return 1.0 - (distance[sourceWordLength][targetWordLength] / (double)Math.Max(source.Length, target.Length));
        }

        private static void DisplaySimilarCommands(string command)
        {
            var suggestions = FindSimilarCommands(command);

            if (!suggestions.Any())
            {
                return;
            }

            Console.WriteLine($"The most similar commands are:");

            foreach (var similarCommand in suggestions)
            {
                Console.WriteLine($"\t'{similarCommand}'");
            }
        }
    }
}
