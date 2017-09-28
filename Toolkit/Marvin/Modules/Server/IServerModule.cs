namespace Marvin.Modules.Server
{
    /// <summary>
    /// Interface for all server modules running within the HeartOfGold application Server.
    /// This interface extends IModule with server specific methods and properties. 
    /// Server modules are intended to do the "heavy lifting" within Marvin. 
    /// The implement behaviour, business logic and data management of each application. 
    /// They might offer web services to connect clients for user interaction. 
    /// Each server module has its own lifecylce within the Runtime. 
    /// However this life cylce might depend on other server modules. 
    /// </summary>
    public interface IServerModule : IModule
    {
        /// <summary>
        /// Start all components and modules to begin execution
        /// </summary>
        void Start();

        /// <summary>
        /// Stop execution, dispose components and return to clean state
        /// </summary>
        void Stop();

        /// <summary>
        /// Console to interact with the module
        /// </summary>
        IServerModuleConsole Console { get; }

        /// <summary>
        /// Access to the modules internal state
        /// </summary>
        IServerModuleState State { get; }
    }
}
