// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Modules.Serialization
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
