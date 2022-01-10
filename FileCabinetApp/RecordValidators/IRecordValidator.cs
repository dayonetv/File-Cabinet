namespace FileCabinetApp.RecordValidators
{
    /// <summary>
    /// Main record validator interface.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Checks records properties correctness.
        /// </summary>
        /// <param name="parameters">Record parameters object to validate.</param>
        public void ValidateParameters(RecordParameters parameters);
    }
}
