using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Load all dependencies of the Hog from the "Dependencies.txt" located next to the HoG.exe.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        /// <summary>
        /// Load all dependencies of the Hog from the "Dependencies.txt" located next to the HoG.exe.
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Dependencies.txt");
            if(!File.Exists(filePath))
                return;

            var dependencies = File.ReadLines(filePath).Where(line => !line.StartsWith("#") && !string.IsNullOrEmpty(line)).ToArray();
            MarvinInstaller.ServicesDependedOn = dependencies;
        }
    }
}
