using System.Data.Entity;
using Devart.Data.PostgreSql;
using Devart.Data.PostgreSql.Entity;

namespace Marvin.Model
{
    /// <summary>
    /// Registers DevArt's DotConnect as EntityFramework provider.
    /// This class has to be referenced by Marvin database models only.
    /// </summary>
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        /// <summary>
        /// This constructor is called directly from EntityFramework.
        /// </summary>
        public EntityFrameworkConfiguration()
        {
            var factory = PgSqlProviderFactory.Instance;
            var services = PgSqlEntityProviderServices.Instance;

            var name = factory.GetType().Namespace;

            SetProviderFactory(name, factory);
            SetProviderServices(name, services);
        }
    }
}