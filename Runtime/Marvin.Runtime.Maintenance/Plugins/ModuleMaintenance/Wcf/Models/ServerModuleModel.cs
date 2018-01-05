using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.Modules;
using Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf.Models;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    /// <summary>
    /// Model for a server module.
    /// </summary>
    [DataContract]
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
        [DataMember]
        public string Name { get; set; } 

        /// <summary>
        /// Health state of the module. See <see cref="ServerModuleState"/> for the states.
        /// </summary>
        [DataMember]
        public ModuleServerModuleState HealthState { get; set; }

        /// <summary>
        /// The start behavior. The module will do this on the start. See <see cref="ModuleStartBehaviour"/> for the behaviors.
        /// </summary>
        [DataMember]
        public ModuleStartBehaviour StartBehaviour { get; set; }

        /// <summary>
        /// The failure behavior of the module. The module will do this when an error occured. See <see cref="FailureBehaviour"/> for the behaviors.
        /// </summary> 
        [DataMember]
        public FailureBehaviour FailureBehaviour { get; set; }

        /// <summary>
        /// Dependencies for this module.
        /// </summary>
        [DataMember]
        public List<ServerModuleModel> Dependencies  { get; set; }

        /// <summary>
        /// Contains a list of notifications for that model.
        /// </summary>
        [DataMember]
        public NotificationModel[] Notifications { get; set; }

        /// <summary>
        /// The configured assembly.
        /// </summary>
        [DataMember]
        public AssemblyModel Assembly { get; set; }
    }

    /// <summary>
    /// Holds importats information about an assembly.
    /// </summary>
    [DataContract]
    public class AssemblyModel
    {
        /// <summary>
        /// Name of the assembly.
        /// </summary>
        [DataMember]
        public string Name { get; set; }   

        /// <summary>
        /// Version of the assembly.
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Name of the bundle where the assembly belongs to.
        /// </summary>
        [DataMember]
        public string Bundle { get; set; }
    }

    /// <summary>
    /// Model for notifications.
    /// </summary>
    [DataContract]
    public class NotificationModel
    {
        /// <summary>
        /// Constructor for a notification.
        /// </summary>
        /// <param name="notification">A notification raisded by a module.</param>
        public NotificationModel(IModuleNotification notification)
        {
            Timestamp = notification.Timestamp;
            //todo: when is a notification important?
            //Important = notification.ImportantNotification;
            Exception = new SerializableException(notification.Exception);
            NotificationType = EnumConverter.Convert(notification.Type);
        }

        /// <summary>
        /// The timestamp when the notofication occured..
        /// </summary>
        [DataMember]
        public DateTime Timestamp { get; set; }
   
        /// <summary>
        /// Flag if this notification is important.
        /// </summary>
        [DataMember]
        public bool Important { get; set; }

        /// <summary>
        /// An exception.
        /// </summary>
        [DataMember]
        public SerializableException Exception { get; set; }

        /// <summary>
        /// Kind of notification
        /// </summary>
        [DataMember]
        public ModuleNotificationType NotificationType { get; set; }
    }
}