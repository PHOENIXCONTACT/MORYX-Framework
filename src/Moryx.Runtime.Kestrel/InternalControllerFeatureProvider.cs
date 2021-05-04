using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Moryx.Runtime.Kestrel
{
    internal class InternalControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly IEnumerable<Type> _ignoreTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        public InternalControllerFeatureProvider(IEnumerable<Type> ignoreServices)
        {
            _ignoreTypes = ignoreServices;
        }

        /// <inheritdoc />
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (_ignoreTypes.Contains(typeInfo))
            {
                return false;
            }

            return !typeInfo.IsAbstract && typeof(Controller).IsAssignableFrom(typeInfo) || base.IsController(typeInfo);
        }
    }
}
