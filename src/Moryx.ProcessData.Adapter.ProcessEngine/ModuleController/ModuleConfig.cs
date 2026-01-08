// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ProcessData.Configuration;

namespace Moryx.ProcessData.Adapter.ProcessEngine;

/// <summary>
/// Module configuration of the adapter <see cref="ModuleController"/>
/// </summary>
[DataContract]
public class ModuleConfig : ConfigBase
{
    /// <summary>
    /// Creates a new instance of the <see cref="ModuleConfig"/>
    /// </summary>
    public ModuleConfig()
    {
        ProcessBindings =
        [
            new() { Name = "productIdent", Binding = "Product.Identifier", ValueTarget = ValueTarget.Tag },
            new() { Name = "productRev", Binding = "Product.Identity.Revision", ValueTarget = ValueTarget.Tag },
            new() { Name = "order", Binding = "Recipe.OrderNumber", ValueTarget = ValueTarget.Tag },
            new() { Name = "operation", Binding = "Recipe.OperationNumber", ValueTarget = ValueTarget.Tag }
        ];

        ActivityBindings =
        [
            new() { Name = "errorCode", Binding = "Tracing.ErrorCode", ValueTarget = ValueTarget.Field },
            new() { Name = "productIdent", Binding = "Product.Identifier", ValueTarget = ValueTarget.Tag },
            new() { Name = "productRev", Binding = "Product.Identity.Revision", ValueTarget = ValueTarget.Tag },
            new() { Name = "order", Binding = "Recipe.OrderNumber", ValueTarget = ValueTarget.Tag },
            new() { Name = "operation", Binding = "Recipe.OperationNumber", ValueTarget = ValueTarget.Tag }
        ];
    }

    /// <summary>
    /// Additional process measurement value bindings
    /// </summary>
    [DataMember]
    [Description("The data bound to this Measurand is gathered on any change of a process within the system.")]
    public List<MeasurementBinding> ProcessBindings { get; set; }

    /// <summary>
    /// Additional activity measurement value bindings
    /// </summary>
    [DataMember]
    [Description("The data bound to this Measurand is gathered on any change of a activity within the system.")]
    public List<MeasurementBinding> ActivityBindings { get; set; }
}