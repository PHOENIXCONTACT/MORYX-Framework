// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Documents;

namespace Moryx.Orders;

/// <summary>
/// Base class for all operation implementations. Will be used for internal and external operations.
/// </summary>
public class Operation
{
    /// <summary>
    /// Referencing order of this operation
    /// </summary>
    public virtual Order Order { get; protected set; }

    /// <summary>
    /// Unique identifier of this operation
    /// </summary>
    public virtual Guid Identifier { get; protected set; }

    /// <summary>
    /// Amount of the operation
    /// </summary>
    public virtual int TotalAmount { get; protected set; }

    /// <summary>
    /// Current target amount of the operation
    /// </summary>
    public virtual int TargetAmount { get; protected set; }

    /// <summary>
    /// Number of the operation e.g. 0030
    /// </summary>
    public virtual string Number { get; protected set; }

    /// <summary>
    /// Name of the operation
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Amount of the operation which is defined as over delivery
    /// </summary>
    public virtual int OverDeliveryAmount { get; protected set; }

    /// <summary>
    /// Amount of the operation which is defined as under delivery
    /// </summary>
    public virtual int UnderDeliveryAmount { get; protected set; }

    /// <summary>
    /// Expected start time of the production
    /// </summary>
    public virtual DateTime PlannedStart { get; protected set; }

    /// <summary>
    /// Expected end time of the production
    /// </summary>
    public virtual DateTime PlannedEnd { get; protected set; }

    /// <summary>
    /// Actual start time
    /// </summary>
    public virtual DateTime? Start { get; protected set; }

    /// <summary>
    /// End of the operation
    /// </summary>
    public virtual DateTime? End { get; protected set; }

    /// <summary>
    /// Target cycle time for the production of the depending product
    /// </summary>
    public virtual double TargetCycleTime { get; protected set; }

    /// <summary>
    /// Target stock for produced parts
    /// </summary>
    public virtual string TargetStock { get; protected set; }

    /// <summary>
    /// Unit of product
    /// </summary>
    public virtual string Unit { get; protected set; }

    /// <summary>
    /// Current sort order of this operation
    /// </summary>
    public virtual int SortOrder { get; protected set; }

    /// <summary>
    /// Product to produce
    /// </summary>
    public virtual ProductType Product { get; protected set; }

    /// <summary>
    /// Represents the progress of this operation
    /// </summary>
    public virtual OperationProgress Progress { get; protected set; }

    /// <summary>
    /// Current state classification of this operation
    /// </summary>
    public virtual OperationStateClassification State
    {
        get => (OperationStateClassification)((int)FullState & 0xFF);
        protected set => FullState = value;
    }

    /// <summary>
    /// Current state classification of this operation
    /// </summary>
    public virtual OperationStateClassification FullState { get; protected set; }

    /// <summary>
    /// Detailed display name of the state
    /// TODO: Remove this property in next major and replace rework OperationStateClassification
    /// </summary>
    public virtual string StateDisplayName { get; protected set; }

    /// <summary>
    /// Source information of the operation
    /// </summary>
    public virtual IOperationSource Source { get; protected set; }

    /// <summary>
    /// Recipe to use in this operation
    /// </summary>
    public virtual IReadOnlyList<IProductRecipe> Recipes { get; protected set; }

    /// <summary>
    /// Current reports of the operation
    /// </summary>
    public virtual IReadOnlyList<OperationReport> Reports { get; protected set; }

    /// <summary>
    /// Current advices of the operation
    /// </summary>
    public virtual IReadOnlyList<OperationAdvice> Advices { get; protected set; }

    /// <summary>
    /// List of parts of the product of this operation
    /// </summary>
    public virtual IReadOnlyList<ProductPart> Parts { get; protected set; }

    /// <summary>
    /// List of the documents which are available for this operation
    /// </summary>
    public virtual IReadOnlyList<Document> Documents { get; protected set; }

    /// <summary>
    /// List of underlying jobs of the operation
    /// </summary>
    public virtual IReadOnlyList<Job> Jobs { get; protected set; }

    /// <summary>
    /// Creation context of this operation which was used for creation
    /// </summary>
    public virtual OperationCreationContext CreationContext { get; protected set; }
}