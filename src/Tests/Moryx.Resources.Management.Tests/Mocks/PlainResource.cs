// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests;

/// <summary>
/// Resource that only implements IResource (via Resource base class) without any more-derived resource interface.
/// </summary>
public class PlainResource : Resource
{
}
