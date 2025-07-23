using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Runtime.Modules;
using Moryx.Users.Management.Facade;

namespace Moryx.Users.Management
{
    /// <summary>
    /// Module controller of the user management module.
    /// </summary>
    [Description("Manages users")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IUserManagement>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "UserManagement";

        /// <inheritdoc />
        public override string Name => ModuleName;

        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager) 
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = dbContextManager;
        }

        /// <summary>
        /// Generic component to manage database contexts
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.ActivateDbContexts(DbContextManager);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<IUserManager>().Start();

            ActivateFacade(_facade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            DeactivateFacade(_facade);

            Container.Resolve<IUserManager>().Stop();
        }

        private readonly UserManagementFacade _facade = new();

        IUserManagement IFacadeContainer<IUserManagement>.Facade => _facade;
    }
}