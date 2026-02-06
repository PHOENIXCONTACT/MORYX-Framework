// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Logging;
using Moryx.Orders;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.FactoryMonitor.Endpoints.Converter;

/// <summary>
/// Provide convertion for resources and models
/// </summary>
internal class Converter
{
    /// <summary>
    /// Logger for this resource
    /// </summary>
    public IModuleLogger Logger { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serialization"></param>

    public Converter(ICustomSerialization serialization, IModuleLogger logger = null)
    {
        Serialization = serialization;
        Logger = logger;
    }

    protected ICustomSerialization Serialization { get; }

    public ResourceChangedModel ToResourceChangedModel(Resource current)
    {
        if (current == null)
        {
            return null;
        }

        var cellEntry = EntryConvert.EncodeObject(current.Descriptor, Serialization);

        return new ResourceChangedModel
        {
            Id = current.Id,
            CellPropertySettings = ToCellPropertySettings(cellEntry, current.GetType()),
            CellName = current.Name,
        };
    }

    public static ActivityChangedModel ToActivityChangedModel(ICell current)
    {
        return current == null
            ? null
            : new ActivityChangedModel
            {
                ResourceId = current.Id,
            };
    }

    public static CellStateChangedModel ToCellStateChangedModel(Resource current)
    {
        return current == null
            ? null
            : new CellStateChangedModel
            {
                Id = current.Id,
            };
    }

    /// <summary>
    /// Return Dictionary of Cell properties with the EntryVisualizationAttribute
    /// </summary>
    /// <param name="cellEntry"> Entry of the cell </param>
    /// <param name="baseType">Type of the resource</param>
    internal Dictionary<string, CellPropertySettings> ToCellPropertySettings(Entry cellEntry, Type baseType)
    {
        if (cellEntry == null)
        {
            return null;
        }

        var cellProperties = new Dictionary<string, CellPropertySettings>();
        if (cellEntry.GetType().IsArray || cellEntry.SubEntries.Count > 0)
        {
            foreach (var subEntry in cellEntry.SubEntries)
            {
                var results = ToCellPropertySettings(subEntry, baseType);
                if (results == null)
                {
                    continue;
                }

                foreach (var kv in results)
                {
                    AddOrReplaceCellProperty(cellProperties, kv.Key, kv.Value, baseType);
                }
            }
            return cellProperties;
        }
        var entryVisualizer = Serialization.GetProperties(baseType)
            .FirstOrDefault(x => x.Name == cellEntry.Identifier)?.GetCustomAttribute<EntryVisualizationAttribute>();
        if (entryVisualizer == null)
        {
            return null;
        }

        var property = CreateCellPropertySettings(cellEntry, entryVisualizer);

        var key = cellEntry.Identifier;

        AddOrReplaceCellProperty(cellProperties, key, property, baseType);
        return cellProperties;
    }

    internal static OrderChangedModel ToOrderChangedModel(Operation operation)
    {
        return new OrderChangedModel
        {
            Order = operation.Order.Number,
            Operation = operation.Number,
            State = GetCorrectOperationState(operation),
        };
    }

    internal static OrderModel ToOrderModel(Operation operation)
    {
        var orderModel = new OrderModel
        {
            Order = operation.Order.Number,
            Operation = operation.Number,
            State = GetCorrectOperationState(operation),
        };
        return orderModel;
    }

    internal static OrderReferenceModel ToOrderReferenceModel(OrderModel orderModel)
    {
        return new OrderReferenceModel
        {
            Order = orderModel?.Order,
            Operation = orderModel?.Operation,
        };
    }
    private void AddOrReplaceCellProperty(Dictionary<string, CellPropertySettings> dictionary, string key, CellPropertySettings value, Type baseType)
    {
        if (dictionary.ContainsKey(key))
        {
            Logger?.LogWarning($"Duplicate key '{key}' in resource '{baseType.Name}'. Overwriting previous value.");
        }
        dictionary[key] = value;
    }
    private static CellPropertySettings CreateCellPropertySettings(Entry entry, EntryVisualizationAttribute entryVisualizer)
    {
        return new CellPropertySettings
        {
            IsDisplayed = false,
            CurrentValue = entry.Value.Current,
            IconName = entryVisualizer?.Icon,
            ValueUnit = entryVisualizer?.Unit,
            DisplayName = entry.DisplayName
        };
    }
    private static InternalOperationClassification GetCorrectOperationState(Operation operation)
    {
        if (operation.Progress.SuccessCount + operation.Progress.FailureCount >= operation.TargetAmount)
        {
            return InternalOperationClassification.AmountReached;
        }
        else
        {
            return (InternalOperationClassification)operation.State;
        }
    }

    /// <summary>
    /// Convert Location to CellLocaitonModel
    /// </summary>
    /// <param name="location"></param>
    /// <returns><see cref="CellLocationModel"/></returns>
    public static CellLocationModel ToCellLocationModel(ILocation location)
    {
        return new CellLocationModel
        {
            Id = location.Id,
            PositionX = location.Position?.PositionX ?? 0,
            PositionY = location.Position?.PositionY ?? 0
        };
    }

    /// <summary>
    /// Convert Transport path to TransportModelPath
    /// </summary>
    /// <param name="transportPath"></param>
    /// <returns><see cref="TransportPathModel"/></returns>
    public static TransportPathModel ToTransportPathModel(ITransportPath transportPath)
    {
        return new TransportPathModel
        {
            Destination = ToCellLocationModel(transportPath.Destination),
            Origin = ToCellLocationModel(transportPath.Origin),
            WayPoints = transportPath.WayPoints
        };
    }

    /// <summary>
    /// Convert TransportPathModel to TransportRouteModel
    /// </summary>
    /// <param name="pathModel"></param>
    /// <returns></returns>
    public static TransportRouteModel ToTransportRouteModel(TransportPathModel pathModel)
    {
        return new TransportRouteModel
        {
            IdCellOfDestination = pathModel.Destination.Id,
            IdCellOfOrigin = pathModel.Origin.Id,
            Paths = pathModel.WayPoints
        };
    }

    public static FactoryStateModel ToFactoryStateModel(IManufacturingFactory factory) => new() { Id = factory.Id };
}
