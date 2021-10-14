using Microsoft.Extensions.Logging;
using Moryx.Runtime.Kernel;
using NLog.Extensions.Logging;

namespace StartProject
{
    /// <summary>
    /// Static program class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// This is the main starting point of this application
        /// </summary>
        public static int Main(string[] args)
        {
            var loader = new HeartOfGold(args);

            var loggerFactory = loader.GlobalContainer.Resolve<ILoggerFactory>();
            loggerFactory.AddProvider(new NLogLoggerProvider());

            var result = loader.Run();
            return (int)result;
        }
    }
}
