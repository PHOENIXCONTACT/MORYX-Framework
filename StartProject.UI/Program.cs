using System;
using Marvin.ClientFramework.Kernel;

namespace StartProject.UI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var heartOfLead = new HeartOfLead(args);
            heartOfLead.Initialize();
            heartOfLead.Start();
        }
    }
}
