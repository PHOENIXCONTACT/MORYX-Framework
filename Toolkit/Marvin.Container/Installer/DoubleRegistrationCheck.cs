using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;

namespace Marvin.Container.Installer
{
    /// <summary>
    /// Class used to check the kernel for an exisiting component registration of this type
    /// </summary>
    public static class DoubleRegistrationCheck
    {
        /// <summary>
        /// Look for an existing component in the kernel
        /// </summary>
        public static bool AllreadyRegistered(IKernel kernel, Type foundComponent, RegistrationAttribute regAtt)
        {
            var name = regAtt == null || string.IsNullOrEmpty(regAtt.Name) ? foundComponent.FullName : regAtt.Name;
            return kernel.HasComponent(name);
        }
    }
}
