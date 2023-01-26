// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Mapper for columns of type <see cref="double"/>
    /// </summary>
    [FloatStrategyConfiguration]
    [Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(FloatColumnMapper))]
    internal class FloatColumnMapper : ColumnMapper<double>
    {
        public FloatColumnMapper(Type targetType) : base(targetType)
        {
        }
    }
}