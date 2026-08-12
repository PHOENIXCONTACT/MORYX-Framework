// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Factory;

// ToDo: Limit Attribute usage in MORYX 12 to reasonable elements and make unit and icon optional
/// <summary>
///  Attribute for a visual representation of the current property inside the Factory monitor UI
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = true)]
public class EntryVisualizationAttribute : Attribute
{
    public EntryVisualizationAttribute(string unit, string icon)
    {
        Unit = unit;
        Icon = icon;
    }

    /// <summary>
    /// Unit of the value for the current property (Ex. Kw/h)
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// Icon to display for this property inside the Factory Monitor UI
    /// </summary>
    public string Icon { get; }
}
