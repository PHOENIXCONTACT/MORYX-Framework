using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using System;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Services
{
    public interface IDatabaseConfigUpdateService
    {
        Type UpdateModel(string targetModel, DatabaseConfigModel config);
    }
}