// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Model.Repositories.Proxy
{
    internal class ParameterPropertyMap
    {
        public ParameterPropertyMap(ParameterInfo parameter, PropertyInfo property)
        {
            Parameter = parameter;
            Property = property;
        }

        public ParameterInfo Parameter { get; }

        public PropertyInfo Property { get; }
    }
}
