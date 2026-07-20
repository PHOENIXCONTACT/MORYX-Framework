// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.Material.IntegrationTests;

internal class MockMaterialContainer : MaterialContainer
{
    public new string? Material
    {
        get => base.Material;
        set => base.Material = value;
    }

    public new double Quantity
    {
        get => base.Quantity;
        set => base.Quantity = value;
    }
}

internal class MockIdentifier : IIdentity
{
    public string Identifier { get; set; }

    public bool Equals(IIdentity? other) => other is not null && Identifier == other.Identifier;

    public void SetIdentifier(string identifier) => Identifier = identifier;
}
