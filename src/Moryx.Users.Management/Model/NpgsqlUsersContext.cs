using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;
using System.IO;

namespace Moryx.Users.Management.Model
{
    /// <inheritdoc />
    [NpgsqlDatabaseContext]
    public class NpgsqlUsersContext : UsersContext
    {
        /// <inheritdoc />
        public NpgsqlUsersContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlUsersContext(DbContextOptions options) : base(options)
        {
        }

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
                var connectionString = configuration.GetConnectionString("Moryx.ControlSystem.Users.Management");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
