// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests;

public interface IExplicitEventResource : IResource
{
    int Bar { get; set; }

    int DoubleBar();

    event EventHandler<int> BarChanged;
}

public class ExplicitEventResource : Resource, IExplicitEventResource
{
    private int _bar;

    int IExplicitEventResource.Bar
    {
        get => _bar;
        set
        {
            _bar = value;
            BarChanged?.Invoke(this, _bar);
        }
    }

    int IExplicitEventResource.DoubleBar()
    {
        _bar *= 2;
        return _bar;
    }

    private event EventHandler<int> BarChanged;

    event EventHandler<int> IExplicitEventResource.BarChanged
    {
        add => BarChanged += value;
        remove => BarChanged -= value;
    }
}
