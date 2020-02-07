// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Maintenance.Modules
{
    /// <summary>
    /// Model for a server module.
    /// </summary>
    public class ServerModuleModel
    {
        /// <summary>
        /// Constructor for a server module mode.
        /// </summary>
        public ServerModuleModel()
        {
            Dependencies = new List<ServerModuleModel>();
            Notifications = new NotificationModel[0];
        }

        /// <summary>
        /// Name of the module.
        /// </summary>
        public string Name { get; set; } 

        /// <summary>
        /// Health state of the module. See <see cref="ServerModuleState"/> for the states.
        /// </summary>
        public ServerModuleState HealthState { get; set; }

        /// <summary>
        /// The start behavior. The module will do this on the start. See <see cref="ModuleStartBehaviour"/> for the behaviors.
        /// </summary>
        public ModuleStartBehaviour StartBehaviour { get; set; }

        /// <summary>
        /// The failure behavior of the module. The module will do this when an error occured. See <see cref="FailureBehaviour"/> for the behaviors.
        /// </summary> 
        public FailureBehaviour FailureBehaviour { get; set; }

        /// <summary>
        /// Dependencies for this module.
        /// </summary>
        public List<ServerModuleModel> Dependencies  { get; set; }

        /// <summary>
        /// Contains a list of notifications for that model.
        /// </summary>
        public NotificationModel[] Notifications { get; set; }

        /// <summary>
        /// The configured assembly.
        /// </summary>
        public AssemblyModel Assembly { get; set; }
    }
}
