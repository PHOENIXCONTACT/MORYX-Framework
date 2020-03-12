// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Castle.Windsor;
using Marvin.Model;

namespace Marvin.Container
{
    internal class GlobalRegistrator : ComponentRegistrator
    {
        /// <summary>
        /// Method to determine if this component shall be installed
        /// </summary>
        public override bool ShallInstall(Type foundType)
        {
            var regAtt = foundType.GetCustomAttribute<RegistrationAttribute>();
            return (regAtt is GlobalComponentAttribute || regAtt is ModelAttribute) && NotRegisteredYet(foundType, regAtt);
        }
    }
}
