// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration;

/// <summary>
/// Empty property provider to pre-fill newly created objects
/// </summary>
public interface IEmptyPropertyProvider
{
    /// <summary>
    /// Fills the object
    /// </summary>
    void FillEmpty(object obj);
}