// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.Factory
{
    /// <summary>
    ///  Attribute for a visual representation of the current property inside the Factory monitor UI
    /// </summary>
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
}
