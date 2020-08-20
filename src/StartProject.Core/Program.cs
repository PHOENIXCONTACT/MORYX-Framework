using Moryx.Runtime.Kernel;

namespace StartProject.Core
{
    class Program
    {
        static int Main(string[] args)
        {
            var loader = new HeartOfGold(args);
            var result = loader.Run();
            return (int)result;
        }
    }
}
