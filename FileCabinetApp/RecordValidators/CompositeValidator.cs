using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Validator that contatins list of several validators to be used.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Current validators to be used.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <inheritdoc/>
        public void ValidateParameters(RecordParameters parameters)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }
        }
    }
}
