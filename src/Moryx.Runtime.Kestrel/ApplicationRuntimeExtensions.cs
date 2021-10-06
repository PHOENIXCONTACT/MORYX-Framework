// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Extensions for DefaultStartup of <see cref="KestrelEndpointHosting"/>
    /// </summary>
    public static class ApplicationRuntimeExtensions
    {

        /// <summary>
        /// Configure Kestrel services
        /// </summary>
        public static IApplicationRuntime UseStartup<TStartup>(this IApplicationRuntime appRuntime)
        {
            return appRuntime.UseStartup(typeof(TStartup));
        }

        /// <summary>
        /// Configure Kestrel services
        /// </summary>
        public static IApplicationRuntime UseStartup(this IApplicationRuntime appRuntime, Type extendedStartup)
        {
            KestrelEndpointHosting.Startup = extendedStartup;
            return appRuntime;
        }
    }
}