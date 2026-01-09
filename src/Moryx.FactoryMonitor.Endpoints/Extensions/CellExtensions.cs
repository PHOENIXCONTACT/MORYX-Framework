// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Recipes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class CellExtensions
{
    extension(ICell cell)
    {
        public ResourceChangedModel GetResourceChangedModel(Converter.Converter converter,
            IResourceManagement resourceManager,
            Func<IMachineLocation, bool> cellFilter)
        {

            var resourceChangedCellModel = resourceManager.ReadUnsafe(cell.Id, converter.ToResourceChangedModel);

            var machineLocation = resourceManager
                .GetResources(cellFilter)
                .FirstOrDefault(x => x.Machine?.Id == cell.Id);
            resourceChangedCellModel.Location = Converter.Converter.ToCellLocationModel(machineLocation);
            resourceChangedCellModel.CellImageURL = machineLocation.Image;
            resourceChangedCellModel.IconName = machineLocation.SpecificIcon;
            resourceChangedCellModel.FactoryId = GetFactoryId(cell, resourceManager);

            return resourceChangedCellModel;
        }

        public long GetFactoryId(IResourceManagement resourceManagement)
        {
            var resource = resourceManagement.ReadUnsafe(cell, x => x.GetFactory());
            return resource?.Id ?? -1;
        }

        public CellStateChangedModel GetCellStateChangedModel(Resource resource)
        {
            var cellStateChangedModel = Converter.Converter.ToCellStateChangedModel(resource);
            cellStateChangedModel.State = GetCellState(cell);
            return cellStateChangedModel;
        }

        public CellStateChangedModel GetCellStateChangedModel(ActivityProgress activityProgress, Resource resource)
        {
            var model = GetCellStateChangedModel(cell, resource);
            model.State = GetCellState(cell, activityProgress);

            return model;
        }

        public ActivityChangedModel GetActivityChangedModel(Activity activity,
            List<OrderModel> orderModels)
        {
            var activityChangedModel = Converter.Converter.ToActivityChangedModel(cell);
            activityChangedModel.Id = activity.Id;

            if (activity.Process is ProductionProcess)
                activityChangedModel.Classification = ActivityClassification.Production;
            else if (activity is IControlSystemActivity controlActivity)
                activityChangedModel.Classification = controlActivity.Classification;

            var recipe = activity.Process.Recipe as IOrderBasedRecipe;
            var orderModel = orderModels.
                SingleOrDefault(o => o.Order == recipe?.OrderNumber && o.Operation == recipe?.OperationNumber);

            activityChangedModel.OrderReferenceModel = Converter.Converter.ToOrderReferenceModel(orderModel);

            return activityChangedModel;
        }

        public CellState GetCellState(ActivityProgress activityProgress)
        {
            var state = GetCellState(cell);
            if (state is CellState.NotReadyToWork)
                return state;

            // Currently we are targetting running activity or completed activity
            // to-do: add more cellstate, state like tearup, teardown etc...
            if (activityProgress is ActivityProgress.Running)
                return CellState.Running;

            return CellState.Idle;
        }

        public CellState GetCellState()
        {
            var currentCapabilities = cell.Capabilities.GetAll();
            if ((currentCapabilities.Count() == 1 && currentCapabilities.Single() is NullCapabilities) || !currentCapabilities.Any())
                return CellState.NotReadyToWork;

            return CellState.Idle;
        }
    }
}