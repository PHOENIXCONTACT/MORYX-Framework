using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Runtime.UserManagement.Model
{
    /// <summary>
    /// Factory to get a unit of work for the UserManagement model
    /// </summary>
    [ModelFactory(UserManagementConstants.Name)]
    public sealed class UserManagementUnitOfWorkFactory : UnitOfWorkFactoryBase<UserManagementContext, NpgsqlModelConfigurator>
    {
        protected override void Configure()
        {
            RegisterRepository<IApplicationAccessRepository>();
            RegisterRepository<IApplicationRepository>();
            RegisterRepository<ILibraryAccessRepository>();
            RegisterRepository<ILibraryRepository>();
            RegisterRepository<IOperationGroupLinkRepository>();
            RegisterRepository<IOperationGroupRepository>();
            RegisterRepository<IOperationRepository>();
            RegisterRepository<IUserGroupRepository>();
        }
    }
}