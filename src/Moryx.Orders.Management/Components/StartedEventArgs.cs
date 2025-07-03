using Moryx.Users;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Event args for started events
    /// </summary>
    internal class StartedEventArgs : OperationEventArgs
    {
        /// <summary>
        /// The user which started the operation
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Creates a new instance of <see cref="StartedEventArgs"/>
        /// </summary>
        public StartedEventArgs(IOperationData operationData, User user) : base(operationData)
        {
            User = user;
        }
    }
}