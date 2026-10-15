// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Serialization;

/// <summary>
/// Stands in for a generated resource class, which is what a localized
/// validation message points at in practice. It has to expose a static
/// property rather than a constant: that is what the framework looks for.
/// </summary>
public static class ValidationMessages
{
    public static string TooShort => "Localized: at least five characters";
}
