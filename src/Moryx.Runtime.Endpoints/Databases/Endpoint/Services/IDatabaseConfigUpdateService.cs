using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Services
{
    public interface IDatabaseConfigUpdateService
    {
        Type UpdateModel(string targetModel, DatabaseConfigModel config);
    }
}