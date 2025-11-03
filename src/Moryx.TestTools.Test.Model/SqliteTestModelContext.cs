using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.Attributes;
using System.IO;
using Moryx.Model.Sqlite;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.TestTools.Test.Model;

[SqliteContext]
[ModelConfigurator(typeof(SqliteModelConfigurator))]
public class SqliteTestModelContext : TestModelContext
{
    public SqliteTestModelContext()
    {
    }

    public SqliteTestModelContext(DbContextOptions options) : base(options)
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
            optionsBuilder.UseSqlite(connectionString);
        }

        optionsBuilder.UseLazyLoadingProxies();
    }
}