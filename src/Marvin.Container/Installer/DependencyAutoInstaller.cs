using System;
using System.Linq;
using System.Reflection;
using Castle.Windsor;

namespace Marvin.Container
{
    /// <summary>
    /// Creates automatically a new instance of the given assembly.
    /// </summary>
    public class DependencyAutoInstaller : AutoInstaller
    {
        private readonly DependencyRegistrationAttribute _depReg;

        /// <summary>
        /// Create new instance for the given assembly
        /// </summary>
        /// <param name="targetAssembly">Assembly to execute installer for</param>
        /// <param name="depReg">Dependency registration attribute</param>
        public DependencyAutoInstaller(Assembly targetAssembly, DependencyRegistrationAttribute depReg) : base(targetAssembly)
        {
            _depReg = depReg;
        }

        /// <summary>
        /// Method to determine if this component shall be installed
        /// </summary>
        protected internal override bool ShallInstall(IComponentRegistrator registrator, Type foundType)
        {
            if (base.ShallInstall(registrator, foundType))
            {
                return _depReg.InstallerMode == InstallerMode.All || TypeRequired(foundType);
            }
            return false;
        }

        private bool TypeRequired(Type foundType)
        {
            // Check if interface was specified as required
            if(foundType.IsInterface && _depReg.RequiredTypes.Contains(foundType))
                return true;

            // Check if class exports required type
            if(foundType.IsClass && _depReg.RequiredTypes.Intersect(ComponentRegistrator.GetComponentServices(foundType)).Any())
                return true;

            return false;    
        }
    }
}