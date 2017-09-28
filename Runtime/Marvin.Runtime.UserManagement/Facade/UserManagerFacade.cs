using System;
using Marvin.Runtime.Base;
using Marvin.Runtime.UserManagement.Wcf;

namespace Marvin.Runtime.UserManagement
{
    internal class UserManagerFacade : UserManagementService, IUserManager, IFacadeControl
    {
        public Action ValidateHealthState { get; set; }

        /// <summary>
        /// Module is starting and facade activated
        /// </summary>
        public void Activate()
        {
        }

        /// <summary>
        /// Module is stopping and facade deactivated
        /// </summary>
        public void Deactivate()
        {
        }
    }
}
