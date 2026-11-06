// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Attributes;

/// <summary>
/// Marks a property as included in the payload when its class
/// is using 'Selective' synchronization mode.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SynchronizableMemberAttribute : Attribute
{

}
