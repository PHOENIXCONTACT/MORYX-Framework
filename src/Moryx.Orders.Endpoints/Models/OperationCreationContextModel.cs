// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using Moryx.Tools;
using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models;

[DataContract]
public class OperationCreationContextModel
{
    /// <summary>
    /// Creates a new instance of the <see cref="OperationCreationContext"/>
    /// </summary>
    public OperationCreationContextModel()
    {

    }

    public OperationCreationContextModel(OperationCreationContext context)
    {
        OrderNumber = context.Order.Number;
        TotalAmount = context.TotalAmount;
        Name = context.Name;
        OperationNumber = context.Number;
        ProductIdentifier = context.ProductIdentifier;
        ProductRevision = context.ProductRevision;
        ProductName = context.ProductName;
        RecipePreselection = context.RecipePreselection;
        OverDeliveryAmount = context.OverDeliveryAmount;
        UnderDeliveryAmount = context.UnderDeliveryAmount;
        PlannedStart = context.PlannedStart;
        PlannedEnd = context.PlannedEnd;
        TargetCycleTime = context.TargetCycleTime;
        Unit = context.Unit;
        TargetStock = context.TargetStock;
        Parts = context.Parts.ToArray();

        if (context.MaterialParameters != null && context.MaterialParameters.Count > 0)
        {
            var materialParameterList = new List<Entry>();
            foreach (var materialParameter in context.MaterialParameters)
            {
                var entry = EntryConvert.EncodeObject(materialParameter);
                materialParameterList.Add(entry);
            }
            MaterialParameters = materialParameterList.ToArray();
        }
    }

    public OperationCreationContext ConvertToContext()
    {
        var context = new OperationCreationContext
        {
            Order = new OrderCreationContext { Number = OrderNumber },
            TotalAmount = TotalAmount,
            Name = Name,
            Number = OperationNumber,
            ProductIdentifier = ProductIdentifier,
            ProductRevision = ProductRevision,
            ProductName = ProductName,
            RecipePreselection = RecipePreselection,
            OverDeliveryAmount = OverDeliveryAmount,
            UnderDeliveryAmount = UnderDeliveryAmount,
            PlannedStart = PlannedStart,
            PlannedEnd = PlannedEnd,
            TargetCycleTime = TargetCycleTime,
            Unit = Unit,
            TargetStock = TargetStock,
            Parts = Parts
        };
        var materialParameterTypes = ReflectionTool.GetPublicClasses<IMaterialParameter>();
        if (MaterialParameters != null && MaterialParameters.Length > 0)
        {
            foreach (var materialEntry in MaterialParameters)
            {
                var type = materialParameterTypes.FirstOrDefault(t => t.Name.Equals(materialEntry.Value.Default));
                if (type == null)
                    continue;
                var materialParameter = (IMaterialParameter)EntryConvert.CreateInstance(type, materialEntry);
                context.MaterialParameters.Add(materialParameter);
            }
        }
        return context;
    }

    /// <summary>
    /// The amount to produce
    /// </summary>
    [DataMember]
    public int TotalAmount { get; set; }

    /// <summary>
    /// Operation name
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// The number of the order this operation belongs to
    /// </summary>
    [DataMember]
    public string OrderNumber { get; set; }

    /// <summary>
    /// The number of this operation
    /// </summary>
    [DataMember]
    public string OperationNumber { get; set; }

    /// <summary>
    /// Main product identifier
    /// </summary>
    [DataMember]
    public string ProductIdentifier { get; set; }

    /// <summary>
    /// Revision of the Product
    /// </summary>
    [DataMember]
    public short ProductRevision { get; set; }

    /// <summary>
    /// Name of the Product
    /// </summary>
    [DataMember]
    public string ProductName { get; set; }

    /// <summary>
    /// Id of the preselected recipe
    /// </summary>
    [DataMember]
    public long RecipePreselection { get; set; }

    /// <summary>
    /// Total amount planned for over delivery
    /// </summary>
    [DataMember]
    public int OverDeliveryAmount { get; set; }

    /// <summary>
    /// Total amount planned for under delivery
    /// </summary>
    [DataMember]
    public int UnderDeliveryAmount { get; set; }

    /// <summary>
    /// Expected start time
    /// </summary>
    [DataMember]
    public DateTime PlannedStart { get; set; }

    /// <summary>
    /// Expected end time
    /// </summary>
    [DataMember]
    public DateTime PlannedEnd { get; set; }

    /// <summary>
    /// Target cycle time for the production of the depending product
    /// </summary>
    [DataMember]
    public double TargetCycleTime { get; set; }

    /// <summary>
    /// Unit of product
    /// </summary>
    [DataMember]
    public string Unit { get; set; }

    /// <summary>
    /// Target stock for produced parts
    /// </summary>
    [DataMember]
    public string TargetStock { get; set; }

    /// <summary>
    /// Part list of the product of this operation
    /// </summary>
    public PartCreationContext[] Parts { get; set; }

    /// <summary>
    /// Additional operation depending parameters for the production
    /// </summary>
    public Entry[] MaterialParameters { get; set; }
}