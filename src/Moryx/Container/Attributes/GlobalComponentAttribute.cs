// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Base attribute for all registrations of the global container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalComponentAttribute : RegistrationAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="lifeStyle">Life style of component</param><param name="services">Implemented service</param>
        public GlobalComponentAttribute(LifeCycle lifeStyle, params Type[] services) : base(lifeStyle, services)
        {
        }
    }
}
