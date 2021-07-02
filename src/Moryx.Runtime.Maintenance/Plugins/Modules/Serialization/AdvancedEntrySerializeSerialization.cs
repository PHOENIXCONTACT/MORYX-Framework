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
        private readonly EntrySerializeSerialization _entrySerializeSerialization;

        /// <inheritdoc />
        public AdvancedEntrySerializeSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider) : base(container, emptyPropertyProvider)
        {
            _entrySerializeSerialization = new EntrySerializeSerialization();
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType) => _entrySerializeSerialization.GetMethods(sourceType);
    }
}
