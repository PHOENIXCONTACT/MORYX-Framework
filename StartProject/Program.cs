using Marvin.Runtime.Kernel;

namespace StartProject
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var loader = new HeartOfGold(args);
            var result = loader.Run();
            return (int)result;
        }
    }
}
