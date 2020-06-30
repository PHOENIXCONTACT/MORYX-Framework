// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Marvin.Tools;

namespace Marvin.Serialization
{
    /// <summary>
    /// Filters methods and properties with <see cref="EditorBrowsableAttribute"/> set.
    /// </summary>
    public class EditorBrowsableSerialization : DefaultSerialization
    {
        private static readonly Lazy<EditorBrowsableSerialization> LazyInstance
            = new Lazy<EditorBrowsableSerialization>(LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EditorBrowsableSerialization Instance => LazyInstance.Value;

        /// <inheritdoc />
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return properties.Where(VerifyEditorBrowsableAttribute);
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            var methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => !m.IsSpecialName);

            methods = Attribute.IsDefined(sourceType, typeof(EditorBrowsableAttribute))
                ? methods.Where(method => method.DeclaringType != typeof(object)) // Filter methods defined by object
                : methods.Where(VerifyEditorBrowsableAttribute); // Filter methods carrying the editor browsable attribute

            return methods;
        }

        /// <inheritdoc />
        public override IEnumerable<ConstructorInfo> GetConstructors(Type sourceType)
        {
            return base.GetConstructors(sourceType).Where(VerifyEditorBrowsableAttribute);
        }

        /// <summary>
        /// Checks if the <see cref="EditorBrowsableAttribute"/> is existent and activated
        /// </summary>
        private static bool VerifyEditorBrowsableAttribute(ICustomAttributeProvider attributeProvider)
        {
            var editorBrowsableAttr = attributeProvider.GetCustomAttribute<EditorBrowsableAttribute>();
            return editorBrowsableAttr != null && editorBrowsableAttr.State != EditorBrowsableState.Never;
        }
    }
}
