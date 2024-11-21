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
        /// <summary>
        /// Instance for <see cref="EntrySerializeSerialization"/> we use to filter properties and methods
        /// </summary>
        private EntrySerializeSerialization _memberFilter = new();

        public ResourceSerialization(IContainer container, IServiceProvider serviceProvider) : base(container, serviceProvider, new ValueProviderExecutor(new ValueProviderExecutorSettings()))
        {
        }

        /// <summary>
        /// Follow the rules for <see cref="EntrySerializeSerialization"/>
        /// </summary>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            return _memberFilter.GetProperties(sourceType);
        }

        /// <summary>
        /// Follow the rules for <see cref="EntrySerializeSerialization"/>
        /// </summary>
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            return _memberFilter.GetMethods(sourceType);
        }

        public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
        {
            return _memberFilter.WriteFilter(sourceType, encoded);
            
        }
    }
}
