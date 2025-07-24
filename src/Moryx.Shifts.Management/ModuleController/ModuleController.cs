﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model;
using Moryx.Operators;
using Moryx.Runtime.Modules;

namespace Moryx.Shifts.Management
{
    /// <summary>
    /// Module controller of the operator management module.
    /// </summary>
    [Description("Manages shifts")]
    public class ModuleController : ServerModuleBase<ModuleConfig>, IFacadeContainer<IShiftManagement>
    {
        /// <summary>
        /// The module's name.
        /// </summary>
        public const string ModuleName = "Shift Management";

        /// <inheritdoc />
        public override string Name => ModuleName;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory, IDbContextManager dbContextManager)
            : base(containerFactory, configManager, loggerFactory)
        {
            DbContextManager = dbContextManager;
        }

        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IResourceManagement ResourceManagement { get; set; }


        [RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
        public IOperatorManagement OperatorManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Generic component to manage database contexts
        /// </summary>
        public IDbContextManager DbContextManager { get; }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container
                .ActivateDbContexts(DbContextManager)
                .SetInstance(ResourceManagement)
                .SetInstance(OperatorManagement);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<IShiftStorage>().Start();
            Container.Resolve<IShiftManager>().Start();

            ActivateFacade(_facade);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            DeactivateFacade(_facade);

            Container.Resolve<IShiftManager>().Stop();
            Container.Resolve<IShiftStorage>().Stop();
        }

        private readonly ShiftManagementFacade _facade = new();

        IShiftManagement IFacadeContainer<IShiftManagement>.Facade => _facade;
    }
}
