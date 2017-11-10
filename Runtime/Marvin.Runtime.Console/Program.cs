using Marvin.Runtime.HeartOfGold;

namespace Marvin.Runtime.Console
{
    /// <summary>
    /// Static programm class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// MEF container
        /// </summary>
        private static HeartOfGoldLoader _loader;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        public static int Main(string[] args)
        {
            _loader = new HeartOfGoldLoader(args);
            var result = _loader.Run();
            return (int)result;
        }
    }
}
