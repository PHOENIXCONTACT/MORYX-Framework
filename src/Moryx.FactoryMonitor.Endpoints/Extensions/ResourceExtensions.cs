// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using System;
using System.Linq;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class ResourceExtensions
    {
        public static Resource GetDisplayableResourceLocation(this Resource machineLocation, ILogger<FactoryMonitorController> logger)
        {
            var location = machineLocation as MachineLocation;
            var results = location.Children.Where(x => x is ICell || x is IManufacturingFactory);
            if (results is null || !results.Any())
            {
                logger.Log(LogLevel.Error, "There is no Resource Type Cell Or " +
                    "ManufacturingFactory found under this Location {Name}", location.Name);
                return null;
            }
            if (results.Count() > 1)
                logger.Log(LogLevel.Warning, "More than one Resource were found under this Location {Name}, " +
                    "The first child will be used", location.Name);

            var machine = results.First();
            if (location is null || machine is null) return null;
            return machineLocation;
        }
        
        public static Resource GetFactory(this Resource resource)
        {
            var parent = resource.Parent;
            if (parent is null) return null;

            if (parent is IManufacturingFactory)
                return parent;
            else
                return GetFactory(parent);
        }
    }
}

