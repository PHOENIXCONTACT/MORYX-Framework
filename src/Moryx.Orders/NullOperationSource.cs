// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Orders;

/// <summary>
/// Empty implementation of a operation source
/// </summary>
[DataContract, DisplayName("Unknown")]
public class NullOperationSource : IOperationSource
{
    /// <inheritdoc />
    [DataMember]
    public string Type => nameof(NullOperationSource);
}
