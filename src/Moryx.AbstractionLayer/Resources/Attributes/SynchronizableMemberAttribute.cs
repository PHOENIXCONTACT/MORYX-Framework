// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Resources.Attributes;

/// <summary>
/// Marks a property as included in the payload when its class
/// is using 'Selective' synchronization mode.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SynchronizableMemberAttribute : Attribute
{
    
}
