using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;
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

    /// <summary>
    /// Name of the database associated with the connection
    /// </summary>
    [Required]
    [DefaultValue("<DatabaseName>")]
    [Description("Name of the database associated with the connection")]
    [ConnectionStringKey("Initial Catalog")]
    public string InitialCatalog { get; set; }

    /// <summary>
    /// User ID to be used when connecting to SQL Server
    /// </summary>
    [DefaultValue("sa")]
    [Description("User ID to be used when connecting to SQL Server")]
    [ConnectionStringKey("User Id") ]
    public string UserId { get; set; }

    /// <summary>
    /// Password to be used when connecting to SQL Server
    /// </summary>
    [Password]
    [DefaultValue("password")]
    [Description("Password to use when connecting to SQL Server")]
    [ConnectionStringKey("Password")]
    public string Password { get; set; }

    /// <summary>
    /// Indicates whether the channel will be encrypted while bypassing walking the certificate chain to validate trust.
    /// </summary>
    [DefaultValue("True")]
    [Description("Indicates whether the channel will be encrypted while bypassing walking the certificate chain to validate trust.")]
    [ConnectionStringKey("TrustServerCertificate")]
    public string TrustServerCertificate { get; set; }
}
