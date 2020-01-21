using System.Data.Entity;
using Npgsql;

namespace Marvin.Model.PostgreSQL
{
    /// <summary>
    /// Registers Npgsql as EntityFramework provider.
    /// This class has to be referenced by Marvin database models only.
    /// </summary>
    public class NpgsqlConfiguration : DbConfiguration
    {
        /// <summary>
        /// This constructor is called directly from EntityFramework.
        /// </summary>
        public NpgsqlConfiguration()
        {
            SetProviderServices("Npgsql", NpgsqlServices.Instance);
            SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
            SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
        }
    }
}