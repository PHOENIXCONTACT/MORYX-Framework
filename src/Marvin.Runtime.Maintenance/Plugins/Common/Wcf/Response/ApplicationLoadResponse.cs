namespace Marvin.Runtime.Maintenance.Plugins.Common
{
    public class ApplicationLoadResponse
    {
        public ulong CPULoad { get; set; }
        public ulong SystemMemory { get; set; }
        public long WorkingSet { get; set; }
    }
}
