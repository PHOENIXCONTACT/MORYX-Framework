// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class ResourceExtensions
{
    extension(Resource resource)
    {
        public Resource GetFactory() => resource.Parent switch
        {
            null => null,
            IManufacturingFactory => resource.Parent,
            _ => GetFactory(resource.Parent),
        };
    }

    extension(IResource resource)
    {
        public ResourceChangedModel GetResourceChangedModel(Converter.Converter converter,
            IResourceManagement resourceManager,
            IMachineLocation machineLocation)
        {
            var resourceChangedCellModel = resourceManager.ReadUnsafe(resource.Id, converter.ToResourceChangedModel);

            resourceChangedCellModel.Location = Converter.Converter.ToCellLocationModel(machineLocation);
            resourceChangedCellModel.CellImageURL = machineLocation.Image;
            resourceChangedCellModel.IconName = machineLocation.SpecificIcon;
            resourceChangedCellModel.FactoryId = GetFactoryId(resource, resourceManager);

            return resourceChangedCellModel;
        }

        public long GetFactoryId(IResourceManagement resourceManagement)
        {
            var factory = resourceManagement.ReadUnsafe(resource, x => x.GetFactory());
            return factory?.Id ?? -1;
        }
    }
}
