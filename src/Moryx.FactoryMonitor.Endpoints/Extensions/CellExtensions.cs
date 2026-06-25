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
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class CellExtensions
{
    extension(ICell cell)
    {
        public CellStateChangedModel GetCellStateChangedModel() => new()
        {
            Id = cell.Id,
            State = GetCellState(cell)
        };

        public CellStateChangedModel GetCellStateChangedModel(ActivityProgress activityProgress) => new()
        {
            Id = cell.Id,
            State = GetCellState(cell, activityProgress)
        };

        public ActivityChangedModel GetActivityChangedModel(Activity activity,
            List<OrderModel> orderModels)
        {
            var activityChangedModel = new ActivityChangedModel
            {
                ResourceId = cell.Id,
                Id = activity.Id
            };

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

        private CellState GetCellState(ActivityProgress activityProgress)
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

        private CellState GetCellState()
        {
            var currentCapabilities = cell.Capabilities.GetAll();
            if ((currentCapabilities.Count() == 1 && currentCapabilities.Single() is NullCapabilities) || !currentCapabilities.Any())
                return CellState.NotReadyToWork;

            return CellState.Idle;
        }
    }
}
