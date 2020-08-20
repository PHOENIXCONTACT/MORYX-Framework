using Moryx.Runtime.Kernel;
using Moryx.Runtime.Wcf;

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
            var result = loader.Run();
            return (int)result;
        }
    }
}
