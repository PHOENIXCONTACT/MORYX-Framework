// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Common interface for <see cref="AssembleActivity"/> and <see cref="AssembleActivity{TParam}"/>
    /// </summary>
    public interface IAssembleActivity : IActivity<AssembleParameters>
    {
    }
}
