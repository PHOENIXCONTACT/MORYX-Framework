namespace Marvin.Communication
{
    /// <summary>
    /// Class to validate and assign binary header instances
    /// </summary>
    public interface IMessageValidator
    {
        /// <summary>
        /// Validate the message
        /// </summary>
        bool Validate(BinaryMessage message);

        /// <summary>
        /// Interpreter used by the protocol of this validator
        /// </summary>
        IMessageInterpreter Interpreter { get; }
    }
}