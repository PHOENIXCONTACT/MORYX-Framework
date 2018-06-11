using Marvin.Products.Model;

namespace Marvin.Products.Samples.Model
{
    public enum OperatingSystem
    {
        Android,
        DosBox,
        Windows2012Server
    }

    public class SmartWatchProductPropertiesEntity : ProductProperties
    {
        public virtual OperatingSystem OperatingSystem { get; set; }
    }
}
