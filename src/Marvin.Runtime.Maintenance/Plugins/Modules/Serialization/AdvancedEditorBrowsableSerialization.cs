// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <inheritdoc />
    internal class AdvancedEditorBrowsableSerialization : PossibleValuesSerialization
    {
        private static readonly EditorBrowsableSerialization EditorBrowsableFilter = new EditorBrowsableSerialization();

        /// <inheritdoc />
        public AdvancedEditorBrowsableSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider) : base(container, emptyPropertyProvider)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            return EditorBrowsableFilter.GetMethods(sourceType);
        }
    }
}
