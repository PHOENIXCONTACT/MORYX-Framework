using System;
using Marvin.Modules;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf.Models
{
    /// <summary>
    /// Helper converter for wcf model enums
    /// </summary>
    public static class EnumConverter
    {
        /// <summary>
        /// Converts from <see cref="ServerModuleState" /> to <see cref="ModuleServerModuleState" />
        /// </summary>
        /// <param name="serverModuleState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ModuleServerModuleState Convert(ServerModuleState serverModuleState)
        {
            switch (serverModuleState)
            {
                case ServerModuleState.Stopped:
                    return ModuleServerModuleState.Stopped;
                case ServerModuleState.Initializing:
                    return ModuleServerModuleState.Initializing;
                case ServerModuleState.Ready:
                    return ModuleServerModuleState.Ready;
                case ServerModuleState.Starting:
                    return ModuleServerModuleState.Starting;
                case ServerModuleState.Running:
                    return ModuleServerModuleState.Running;
                case ServerModuleState.Stopping:
                    return ModuleServerModuleState.Stopping;
                case ServerModuleState.Failure:
                    return ModuleServerModuleState.Failure;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverModuleState), serverModuleState, null);
            }
        }

        /// <summary>
        /// Converts from <see cref="NotificationType" /> to <see cref="ModuleNotificationType" />
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ModuleNotificationType Convert(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Info:
                    return ModuleNotificationType.Info;
                case NotificationType.Warning:
                    return ModuleNotificationType.Warning;
                case NotificationType.Failure:
                    return ModuleNotificationType.Failure;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null);
            }
        }
    }
}
