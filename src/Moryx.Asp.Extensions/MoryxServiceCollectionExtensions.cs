// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;

namespace Moryx.Asp.Integration
{
    public static class MoryxServiceCollectionExtensions
    {
        public static void AddMoryxFacades(this IServiceCollection serviceCollection, IApplicationRuntime runtime)
        {
            var facadeCollector = runtime.GlobalContainer.Resolve<IFacadeCollector>();
            var facades = facadeCollector.Facades;
            foreach (var facade in facades)
            {
                // Register facade for all its interfaces
                foreach (var facadeApi in facade.GetType().GetInterfaces())
                {
                    serviceCollection.AddSingleton(facadeApi, facade);
                }
            }
        }
    }
}
