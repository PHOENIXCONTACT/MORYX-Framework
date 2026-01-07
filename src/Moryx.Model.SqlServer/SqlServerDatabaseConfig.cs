using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Database config for the SqlServer databases
/// </summary>
[DataContract]
public class SqlServerDatabaseConfig : DatabaseConfig
{
    [DataMember, Required]
    [ConnectionStringKey("Initial Catalog")]
    public string InitialCatalog { get; set; }

    [DataMember]
    [ConnectionStringKey("User Id")]
    public string UserId { get; set; }

    [DataMember]
    [ConnectionStringKey("Password")]
    public string CacheMode { get; set; }
    
    [DataMember]
    [ConnectionStringKey("TrustServerCertificate")]
    public bool TrustServerCertificate { get; set; }
}
