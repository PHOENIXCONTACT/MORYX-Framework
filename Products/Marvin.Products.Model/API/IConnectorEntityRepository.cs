using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ConnectorEntity repository.
    /// </summary>
    public interface IConnectorEntityRepository : IRepository<ConnectorEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ConnectorEntity Create(long connectorId, int classification); 
    }
}
