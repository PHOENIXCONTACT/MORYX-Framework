// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public interface IDetailsComponent : IEditModeViewModel, IDetailsViewModel
    {
        bool InitializeCalled { get; }
    }
}
