namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Exception thrown by the resource proxy fetched from the facade if it was detached and can no longer
    /// interact with the target instance.
    /// </summary>
    public class ProxyDetachedException : MarvinException
    {
        /// <summary>
        /// Create new instance of the detached excpetion
        /// </summary>
        public ProxyDetachedException() : base("The proxy was detached and can no longer be used!")
        {
        }
    }
}