// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

/// <summary>
/// Strategy to resolve array index for a certain mapping value
/// </summary>
public interface IIndexResolver
{
    /// <summary>
    /// Resolve index by mapping value
    /// </summary>
    int Resolve(long mappingValue);
}