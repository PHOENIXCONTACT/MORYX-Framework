// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Recipes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class CellExtensions
    {
        public static ResourceChangedModel GetResourceChangedModel(
           this ICell cell,
           Converter converter,
           IResourceManagement resourceManager,
           Func<IMachineLocation, bool> cellFilter)
        {

            var resourceChangedCellModel = resourceManager.Read(cell.Id, converter.ToResourceChangedModel);

            var machineLocation = resourceManager
                .GetResources(cellFilter)
                .FirstOrDefault(x => x.Machine?.Id == cell.Id);
            resourceChangedCellModel.Location = Converter.ToCellLocationModel(machineLocation);
            resourceChangedCellModel.CellImageURL = machineLocation.Image;
            resourceChangedCellModel.IconName = machineLocation.SpecificIcon;
            resourceChangedCellModel.FactoryId = GetFactoryId(cell, resourceManager);

            return resourceChangedCellModel;
        }



        public static long GetFactoryId(this ICell cell, IResourceManagement resourceManagement)
        {
            var resource = resourceManagement.Read(cell, x => x.GetFactory());
            return resource?.Id ?? -1;
        }

        public static CellStateChangedModel GetCellStateChangedModel(this ICell cell, Resource resource)
        {
            var cellStateChangedModel = Converter.ToCellStateChangedModel(resource);
            cellStateChangedModel.State = GetCellState(cell);
            return cellStateChangedModel;
        }

        public static CellStateChangedModel GetCellStateChangedModel(this ICell cell, ActivityProgress activityProgress, Resource resource)
        {
            var model = GetCellStateChangedModel(cell, resource);
            model.State = GetCellState(cell, activityProgress);

            return model;
        }

        public static ActivityChangedModel GetActivityChangedModel(
            this ICell cell,
            IActivity activity,
            List<OrderModel> orderModels)
        {
            var activityChangedModel = Converter.ToActivityChangedModel(cell);
            activityChangedModel.Id = activity.Id;

            if (activity.Process is ProductionProcess)
                activityChangedModel.Classification = ActivityClassification.Production;
            else if (activity is IControlSystemActivity controlActivity)
                activityChangedModel.Classification = controlActivity.Classification;

            var recipe = activity.Process.Recipe as IOrderBasedRecipe;
            var orderModel = orderModels.
                SingleOrDefault(o => o.Order == recipe?.OrderNumber && o.Operation == recipe?.OperationNumber);

            activityChangedModel.OrderReferenceModel = Converter.ToOrderReferenceModel(orderModel);

            return activityChangedModel;
        }

        public static CellState GetCellState(this ICell cell, ActivityProgress activityProgress)
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

        public static CellState GetCellState(this ICell cell)
        {
            var currentCapabilities = cell.Capabilities.GetAll();
            if ((currentCapabilities.Count() == 1 && currentCapabilities.Single() is NullCapabilities) || !currentCapabilities.Any())
                return CellState.NotReadyToWork;

            return CellState.Idle;
        }


    }
}

