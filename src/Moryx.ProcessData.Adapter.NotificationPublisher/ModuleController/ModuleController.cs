// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Notifications;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.NotificationPublisher
{
    /// <summary>
    /// Module controller of the process data monitor adapter.
    /// </summary>
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        /// <inheritdoc />
        public override string Name => "PdmNotificationPublisher";

        /// <summary>
        /// NotificationPublisher facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public INotificationPublisher NotificationPublisher { get; set; }

        /// <summary>
        /// ProcessDataMonitor facade dependency
        /// </summary>
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        /// <summary>
        /// Create instance of notification adapter
        /// </summary>
        public ModuleController(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Container.SetInstance(NotificationPublisher)
                .SetInstance(ProcessDataMonitor);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            Container.Resolve<NotificationPublisherAdapter>().Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            Container.Resolve<NotificationPublisherAdapter>().Stop();
        }
    }
}
