using Marvin.Container;

namespace Marvin.Products.Management.Modification
{
    [PluginFactory]
    internal interface IProductConverterFactory
    {
        /// <summary>
        /// Create converter instance
        /// </summary>
        /// <returns></returns>
        IProductConverter Create();

        /// <summary>
        /// Destroy instance after usage
        /// </summary>
        /// <param name="instance"></param>
        void Destroy(IProductConverter instance);
    }
}