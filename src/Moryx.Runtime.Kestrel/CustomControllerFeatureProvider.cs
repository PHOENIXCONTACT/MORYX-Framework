using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Custom override to allow internal containers and only controllers which are inherit <see cref="Controller"/>
    /// </summary>
    internal class CustomControllerFeatureProvider : ControllerFeatureProvider
    {
        private const string ControllerTypeNameSuffix = "Controller";

        protected override bool IsController(TypeInfo typeInfo)
        {
            // Ignore interfaces
            if (!typeInfo.IsClass)
                return false;

            // Ignore abstracts
            if (typeInfo.IsAbstract)
                return false;

            // Ignore generics
            if (typeInfo.ContainsGenericParameters)
                return false;

            // Ignore none controllers
            if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
                return false;

            // Controller must end with suffix OR must have the controller attribute
            if (!typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) &&
                !typeInfo.IsDefined(typeof(ControllerAttribute)))
                return false;

            // Allow only Controllers
            if (!typeof(Controller).IsAssignableFrom(typeInfo))
                return false;

            return true;
        }
    }
}