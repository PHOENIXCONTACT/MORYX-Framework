using Marvin.Products.Model;

namespace Marvin.Products.Samples.Model
{
    public enum OperatingSystem
    {
        Android,
        DosBox,
        Windows2012Server
    }

    /// <summary>
    /// Product entity
    /// </summary>
    public class SmartWatchProductPropertiesEntity : ProductProperties
    {
        /// <summary>
        /// OperatingSystem
        /// </summary>
        public virtual OperatingSystem OperatingSystem { get; set; }
    }
}
