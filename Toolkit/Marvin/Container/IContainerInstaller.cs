namespace Marvin.Container
{
    /// <summary>
    /// Interface for all installers that extend the <see cref="IContainer"/>
    /// </summary>
    public interface IContainerInstaller
    {
        /// <summary>
        /// Install using the given registrator
        /// </summary>
        void Install(IComponentRegistrator registrator);
    }
}
