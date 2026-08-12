// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Tests;

internal sealed class MockOrderLinkedMaterialContainer : OrderLinkedMaterialContainer
{
    public int ValidationErrorHandlingCount { get; private set; }

    protected override Task HandleValidationErrors(ValidationContext? context)
    {
        ValidationErrorHandlingCount++;
        return Task.CompletedTask;
    }
}
