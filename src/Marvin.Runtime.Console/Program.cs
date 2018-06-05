using Marvin.Runtime.Kernel;

namespace Marvin.Runtime.Console
{
    /// <summary>
    /// Static programm class
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
