// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Serialization
{
    /// <summary>
    /// Attribute to decorate classes, methods or properties that can be called from the UI.
    /// If this is used on a class, all public methods and properties are considered "EditorVisible"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class EditorVisibleAttribute : Attribute
    {
    }
}
