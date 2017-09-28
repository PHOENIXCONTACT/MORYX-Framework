using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Marvin.Base.CastleTools.Installer;
using Marvin.UserManagement.ManagementService.UserManagement;

namespace Marvin.UserManagement.ManagementService
{
    internal class UserManagementServiceInstaller : AutoInstaller
    {
    }
}
