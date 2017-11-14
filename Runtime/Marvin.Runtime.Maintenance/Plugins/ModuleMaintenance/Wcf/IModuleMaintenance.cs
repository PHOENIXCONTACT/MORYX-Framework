using System.Collections.Generic;
using System.ServiceModel;
using Marvin.Runtime.Modules;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    /// <summary>
    /// Base service contract for the maintenance module.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.3.0.0", MinClientVersion = "1.3.0.0")]
    public interface IModuleMaintenance
    {
        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
        [OperationContract]
        List<ServerModuleModel> GetAll();

        /// <summary>
        /// Gets the dependency evaluation.
        /// </summary>
        /// <returns>The dependency evaluation.</returns>
        [OperationContract]
        DependencyEvaluation GetDependencyEvaluation();

        /// <summary>
        /// Try to start the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be started.</param>
        [OperationContract]
        void Start(string moduleName);

        /// <summary>
        /// Try to stop the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be stopper.</param>
        [OperationContract]
        void Stop(string moduleName);

        /// <summary>
        /// Try to reincarnate the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be reincarnated.</param>
        [OperationContract]
        void Reincarnate(string moduleName);

        /// <summary>
        /// Confirms the warning of the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module where the warning will confirmed.</param>
        [OperationContract]
        void ConfirmWarning(string moduleName);

        /// <summary>
        /// Get the config for the module from the moduleName.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>Configuration of the requested module.</returns>
        [OperationContract]
        Config GetConfig(string moduleName);

        /// <summary>
        /// Set the given config and react to updated mode.
        /// </summary>
        /// <param name="model">The configuration which will be saved.</param>
        /// <param name="updateMode">Mode how to react after the save of the config. See <see cref="ConfigUpdateMode"/> for details.</param>
        [OperationContract]
        void SetConfig(Config model, ConfigUpdateMode updateMode);

        /// <summary>
        /// Set the start behavior of the given module.
        /// </summary>
        /// <param name="moduleName">The name of the module where the start behavior will be set.</param>
        /// <param name="startBehaviour">The new start behavior.</param>
        [OperationContract]
        void SetStartBehaviour(string moduleName, ModuleStartBehaviour startBehaviour);

        /// <summary>
        /// Set the failure behavior of the given module.
        /// </summary>
        /// <param name="moduleName">Name of the module where the failure behavior will be set.</param>
        /// <param name="failureBehaviour">The new failure behavior.</param>
        [OperationContract]
        void SetFailureBehaviour(string moduleName, FailureBehaviour failureBehaviour);
    }
}
