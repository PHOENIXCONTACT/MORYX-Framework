// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Extension methods to use setup the MORYX AccessManagement within an Asp.Net Core application 
    /// using the created host of the application.
    /// </summary>
    public static class HostExtensions
    {
        private static readonly string[] _permissions = {
            "Moryx.Runtime.Database.CanView",
            "Moryx.Runtime.Database.CanSetAndTestConfig",
            "Moryx.Runtime.Database.CanCreate",
            "Moryx.Runtime.Database.CanErase",
            "Moryx.Runtime.Database.CanDumpAndRestore",
            "Moryx.Runtime.Database.CanMigrateModel",
            "Moryx.Runtime.Database.CanSetup",
            "Moryx.Runtime.Modules.CanView",
            "Moryx.Runtime.Modules.CanViewConfiguration",
            "Moryx.Runtime.Modules.CanViewMethods",
            "Moryx.Runtime.Modules.CanControl",
            "Moryx.Runtime.Modules.CanConfigure",
            "Moryx.Runtime.Modules.CanConfirmNotifications",
            "Moryx.Runtime.Modules.CanInvoke",
            "Moryx.Runtime.Common.CanGetGeneralInformation",
            "Moryx.Resources.CanViewTree",
            "Moryx.Resources.CanViewDetails",
            "Moryx.Resources.CanEdit",
            "Moryx.Resources.CanAddResource",
            "Moryx.Resources.CanAdd",
            "Moryx.Resources.CanDelete",
            "Moryx.Resources.CanInvokeMethod",
            "Moryx.Products.CanViewTypes",
            "Moryx.Products.CanCreateAndEditRecipes",
            "Moryx.Products.CanEditType",
            "Moryx.Products.CanDuplicateType",
            "Moryx.Products.CanImport",
            "Moryx.Products.CanDeleteType",
            "Moryx.Products.CanViewInstances",
            "Moryx.Products.CanCreateInstances",
            "Moryx.Workplans.CanView",
            "Moryx.Workplans.CanEdit",
            "Moryx.Workplans.CanDelete",
            "Moryx.Media.CanView",
            "Moryx.Media.CanAdd",
            "Moryx.Media.CanRemove",
            "Moryx.Jobs.CanView",
            "Moryx.Jobs.CanComplete",
            "Moryx.Jobs.CanAbort",
            "Moryx.Processes.CanView",
            "Moryx.VisualInstruction.CanView",
            "Moryx.VisualInstruction.CanComplete",
            "Moryx.VisualInstruction.CanAdd",
            "Moryx.VisualInstruction.CanClear",
            "Moryx.Notifications.CanView",
            "Moryx.Notifications.CanAcknowledge",
            "Moryx.Orders.CanView",
            "Moryx.Orders.CanViewDocuments",
            "Moryx.Orders.CanAdd",
            "Moryx.Orders.CanManage",
            "Moryx.Orders.CanBegin",
            "Moryx.Orders.CanInterrupt",
            "Moryx.Orders.CanReport",
            "Moryx.Orders.CanAdvice",
            "Moryx.FactoryMonitor.CanView",
            "Moryx.Analytics.CanView",
            "Moryx.CommandCenter.CanView",
            "Moryx.Skills.CanView",
            "Moryx.Skills.CanManage",
            "Moryx.Operators.CanView",
            "Moryx.Operators.CanManage"
        };

        /// <summary>
        /// Extension method to apply all migrations to the <see cref="MoryxIdentitiesDbContext"/> of the application.
        /// </summary>
        /// <param name="host">The Microsoft.Extensions.Hosting.IHost holding the <see cref="MoryxIdentitiesDbContext"/>.</param>
        public static IHost Migrate(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<MoryxIdentitiesDbContext>();
            context.Database.Migrate();

            return host;
        }

        /// <summary>
        /// Seeds an empty <see cref="MoryxIdentitiesDbContext"/> with a SuperAdmin role, an admin user and all currently known MORYX permissions.
        /// </summary>
        /// <param name="host">The Microsoft.Extensions.Hosting.IHost holding the <see cref="MoryxIdentitiesDbContext"/>.</param>
        /// <param name="additionalPermissions">Additional permissions to add to the database with the initial seeding.</param>
        /// <remarks>Requires an existing database to be seeded. A call of the <see cref="Migrate(IHost)"/> method is usually
        /// used to make sure of this.</remarks>
        public static async Task<IHost> Seed(this IHost host, IEnumerable<string> additionalPermissions)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("app");

            var userManager = services.GetRequiredService<MoryxUserManager>();
            var roleManager = services.GetRequiredService<MoryxRoleManager>();
            var permissionManager = services.GetRequiredService<IPermissionManager>();

            var seedResult = await SeedDatabase(userManager, roleManager, permissionManager, logger, additionalPermissions);

            return host;
        }

        /// <summary>
        /// Seeds an empty <see cref="MoryxIdentitiesDbContext"/> with a SuperAdmin role, an admin user and all currently known MORYX permissions.
        /// </summary>
        /// <param name="host">The Microsoft.Extensions.Hosting.IHost holding the <see cref="MoryxIdentitiesDbContext"/>.</param>
        /// <remarks>Requires an existing database to be seeded. A call of the <see cref="Migrate(IHost)"/> method is usually
        /// used to make sure of this.</remarks>
        public static async Task<IHost> Seed(this IHost host) => await host.Seed([]);

        private static async Task<bool> SeedDatabase(MoryxUserManager userManager,
                                                     MoryxRoleManager roleManager,
                                                     IPermissionManager permissionManager,
                                                     ILogger logger,
                                                     IEnumerable<string> additionalPermissions)
        {
            // Seed super admin role
            var role = await roleManager.FindByNameAsync(Roles.SuperAdmin);
            if (role == null)
            {
                var roleCreationResult = await roleManager.CreateAsync(new MoryxRole(Roles.SuperAdmin));
                if (!roleCreationResult.Succeeded)
                {
                    logger.LogError("Super admin role could not be created");
                    foreach (var error in roleCreationResult.Errors)
                        logger.LogError("{0}: {1}", error.Code, error.Description);

                    return false;
                }
            }
            else
            {
                logger.LogWarning($"{Roles.SuperAdmin} role already exists in the database");
                return true;
            }

            //Seed Default User
            var defaultUser = new MoryxUser
            {
                UserName = "admin",
                Email = "admin@sample.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };


            var user = await userManager.FindByNameAsync(defaultUser.UserName);
            if (user == null)
            {
                var userResult = await userManager.CreateAsync(defaultUser, "Admin1!");

                if (!userResult.Succeeded)
                {
                    logger.LogError("Admin user could not be created");
                    foreach (var error in userResult.Errors)
                        logger.LogError("{0}: {1}", error.Code, error.Description);

                    return false;
                }

                await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin);
            }
            else
            {
                logger.LogWarning("Admin already existent in database");
                return true;
            }

            foreach (var permission in additionalPermissions.Concat(_permissions))
            {
                var permissionResult = await permissionManager.CreateAsync(permission);
                if (!permissionResult.Succeeded)
                {
                    logger.LogError("Permission '{0}' could not be created", permission);
                    foreach (var error in permissionResult.Errors)
                        logger.LogError("{0}: {1}", error.Code, error.Description);
                }
            }

            return true;
        }
    }
}

