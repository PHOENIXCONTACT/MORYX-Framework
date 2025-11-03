// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model.Attributes
{
    /// <summary>
    /// Attribute to identify database specific contexts
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseSpecificContextAttribute : Attribute
    {
        
    }
}
