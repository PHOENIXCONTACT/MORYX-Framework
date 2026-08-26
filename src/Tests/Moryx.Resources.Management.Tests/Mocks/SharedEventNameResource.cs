// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests;

public interface IFirstEventSource : IResource
{
    event EventHandler<int> StatusChanged;

    void RaiseFirst(int value);
}

public interface ISecondEventSource : IResource
{
    event EventHandler<int> StatusChanged;

    void RaiseSecond(int value);
}

public class SharedEventNameResource : Resource, IFirstEventSource, ISecondEventSource
{
    private event EventHandler<int> FirstStatusChanged;
    private event EventHandler<int> SecondStatusChanged;

    event EventHandler<int> IFirstEventSource.StatusChanged
    {
        add => FirstStatusChanged += value;
        remove => FirstStatusChanged -= value;
    }

    event EventHandler<int> ISecondEventSource.StatusChanged
    {
        add => SecondStatusChanged += value;
        remove => SecondStatusChanged -= value;
    }

    public void RaiseFirst(int value) =>
        FirstStatusChanged?.Invoke(this, value);

    public void RaiseSecond(int value) =>
        SecondStatusChanged?.Invoke(this, value);
}
