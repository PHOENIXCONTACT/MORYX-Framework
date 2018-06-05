namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Interface for all modules offering an API Facade
    /// </summary>
    /// <typeparam name="TFacade">Type of facade offered by this module</typeparam>
    public interface IFacadeContainer<out TFacade>
    {
        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        /// <remarks>
        /// The hard-coded name of this property is also used in Marvin.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
        /// </remarks>
        TFacade Facade { get; }
    }
}