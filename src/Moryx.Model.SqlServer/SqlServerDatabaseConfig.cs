using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Database config for the SqlServer databases
/// </summary>
[DataContract]
public class SqlServerDatabaseConfig : DatabaseConfig<SqlServerDatabaseConnectionSettings>
{
    /// <summary>
    /// Creates a new instance of the <see cref="SqlServerDatabaseConfig"/>
    /// </summary>
    public SqlServerDatabaseConfig()
    {
        ConnectionSettings = new SqlServerDatabaseConnectionSettings();
        ConfiguratorTypename = typeof(SqlServerModelConfigurator).AssemblyQualifiedName;
    }
}

/// <summary>
/// Database connection settings for the SqlServer databases
/// </summary>
public class SqlServerDatabaseConnectionSettings : DatabaseConnectionSettings
{
    private string _database;

    /// <inheritdoc />
    [DataMember]
    public override string Database
    {
        get => _database;
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            _database = value;
            ConnectionString = ConnectionString?.Replace("<DatabaseName>", value);
        }
    }

    /// <inheritdoc/>
    [DataMember, Required, DefaultValue("Server=localhost;Initial Catalog=<DatabaseName>;User Id=sa;Password=password;TrustServerCertificate=True;")]
    public override string ConnectionString { get; set; }
}