// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization
{
    /// <summary>
    /// Wrapper for property on the object instance
    /// </summary>
    internal struct PropertyInstanceWrapper
    {
        /// <summary>
        /// Type wrapper
        /// </summary>
        private readonly PropertyTypeWrapper _typeWrapper;

        /// <summary>
        /// Config entry that represents this property
        /// </summary>
        private readonly Entry _entry;

        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public PropertyInstanceWrapper(PropertyTypeWrapper typeWrapper, Entry entry)
        {
            _typeWrapper = typeWrapper;
            _entry = entry;
        }

        /// <summary>
        /// Set value of the property on the target
        /// </summary>
        /// <param name="target">Target object</param>
        public void SetValue(object target)
        {
            _typeWrapper.SetValue(target, _entry);
        }

        /// <summary>
        /// Read value from object and write to config
        /// </summary>
        public void ReadValue(object source)
        {
            _typeWrapper.ReadValue(source, _entry);
        }
    }
}
