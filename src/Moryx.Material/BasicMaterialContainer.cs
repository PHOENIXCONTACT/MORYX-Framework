// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Material;

/// <summary>
/// Ready-to-use, non-abstract <see cref="MaterialContainer"/> for the most common cases.
/// Application engineers may register this resource type directly without subclassing.
/// </summary>
[Display(Name = "Basic Material Container", Description = "Default container resource holding a single material with a quantity.")]
public class BasicMaterialContainer : MaterialContainer, IMaterialContainer
{
}
