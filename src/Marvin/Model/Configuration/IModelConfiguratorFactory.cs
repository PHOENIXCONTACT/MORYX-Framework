namespace Marvin.Model
{
    /// <summary>
    /// Interface to create a <see cref="IModelConfigurator"/>. 
    /// Normaly implemented by the <see cref="IUnitOfWorkFactory"/>
    /// </summary>
    public interface IModelConfiguratorFactory
    {
        /// <summary>
        /// Gets the current model configurator for the model
        /// </summary>
        IModelConfigurator GetConfigurator();
    }
}