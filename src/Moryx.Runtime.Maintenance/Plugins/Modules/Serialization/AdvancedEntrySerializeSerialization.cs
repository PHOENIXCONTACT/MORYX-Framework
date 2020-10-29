// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    /// <inheritdoc />
    internal class AdvancedEntrySerializeSerialization : PossibleValuesSerialization
    {
        /// <inheritdoc />
        public AdvancedEntrySerializeSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider) : base(container, emptyPropertyProvider)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType) => EntrySerializeSerialization.Instance.GetMethods(sourceType);
    }
}
