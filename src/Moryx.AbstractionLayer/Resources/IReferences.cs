// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Interface for reference collections. This is necessary to keep Castle from interfering
/// </summary>
public interface IReferences<TResource> : ICollection<TResource>, IReferenceCollection
    where TResource : IResource
{
}