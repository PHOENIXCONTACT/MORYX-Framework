using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model;
using System.IO;

namespace Moryx.Users.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public class UsersContext : MoryxDbContext
    {
        /// <inheritdoc />
        public UsersContext()
        {
        }

        /// <inheritdoc />
        public UsersContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// There are no comments for <see cref="UserEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<UserEntity> UserEntities { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("Moryx.ControlSystem.MachineConnector.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }

            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
