// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    internal class ResourceSerialization : PossibleValuesSerialization
    {
        public ResourceSerialization(IContainer container) : base(container, new ValueProviderExecutor(new ValueProviderExecutorSettings()))
        {
        }

        /// <summary>
        /// Only export properties flagged with <see cref="EntrySerializeAttribute"/>
        /// </summary>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            return typeof(Resource).IsAssignableFrom(sourceType)
                ? base.GetProperties(sourceType).Where(p => p.GetCustomAttribute<EntrySerializeAttribute>()?.Mode == EntrySerializeMode.Always)
                : new EntrySerializeSerialization().GetProperties(sourceType);
        }

        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            return new EntrySerializeSerialization().GetMethods(sourceType);
        }
    }
}
