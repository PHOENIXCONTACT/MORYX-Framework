using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.Attributes;
using System.IO;
using Moryx.Model.PostgreSQL;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.TestTools.Test.Model;

[NpgsqlDatabaseContext]
[ModelConfigurator(typeof(NpgsqlModelConfigurator))]
public class NpgsqlTestModelContext : TestModelContext
{
    public NpgsqlTestModelContext()
    {
    }

    public NpgsqlTestModelContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("Moryx.TestTools.Test.Model");
            optionsBuilder.UseNpgsql(connectionString);
        }

        optionsBuilder.UseLazyLoadingProxies();
    }
}