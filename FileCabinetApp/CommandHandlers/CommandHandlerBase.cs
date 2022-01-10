using System;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for the basic implementation of command handler methods.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <inheritdoc/>
        public virtual void Handle(AppCommandRequest request)
        {
            if (request != null)
            {
                this.nextHandler?.Handle(request);
            }
        }

        /// <inheritdoc/>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return commandHandler;
        }

        /// <summary>
        /// Reads user string inputs.
        /// </summary>
        /// <typeparam name="T">returnong value type.</typeparam>
        /// <param name="converter">converter from string to any type.</param>
        /// <param name="validator">input validator.</param>
        /// <returns>Converted value from the string.</returns>
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Converts input to string.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, string> StringConverter(string input)
        {
            input = input?.Trim();
            bool result = !string.IsNullOrEmpty(input);

            return new Tuple<bool, string, string>(result, input, input);
        }

        /// <summary>
        /// Converts input to char.
        /// </summary>
        /// <param name="input">input string to convert.</param>
        /// <returns>converting success, converting message, converting result. </returns>
        protected static Tuple<bool, string, char> CharConverter(string input)
        {
            input = input?.Trim();
            bool result = char.TryParse(input, out char symbol);

            return new Tuple<bool, string, char>(result, input, symbol);
        }

        /// <summary>
        /// Gets property from the type by property name.
        /// </summary>
        /// <param name="propertyName"><see cref="string"/>property name.</param>
        /// <param name="targetType"><see cref="Type"/>type to get properties from.</param>
        /// <returns>Finded property. (null if property is not found).</returns>
        protected static PropertyInfo GetProperty(string propertyName, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            PropertyInfo property = Array.Find(targetType.GetProperties(), (property) => property.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            return property;
        }
    }
}
