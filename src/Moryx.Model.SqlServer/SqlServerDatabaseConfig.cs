using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Model.Attributes;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Database config for the SqlServer databases
/// </summary>
[DataContract]
public class SqlServerDatabaseConfig : DatabaseConfig
{
    /// <inheritdoc />
    public override string ConfiguratorType => typeof(SqlServerModelConfigurator).AssemblyQualifiedName;

    [Required, DefaultValue("<DatabaseName>")]
    [ConnectionStringKey("Initial Catalog")]
    public string InitialCatalog { get; set; }

    [ConnectionStringKey("User Id") , DefaultValue("sa")]
    public string UserId { get; set; }

    [ConnectionStringKey("Password"), DefaultValue("password")]
    public string Password { get; set; }

    [ConnectionStringKey("TrustServerCertificate"), DefaultValue("True")]
    public string TrustServerCertificate { get; set; }
}
