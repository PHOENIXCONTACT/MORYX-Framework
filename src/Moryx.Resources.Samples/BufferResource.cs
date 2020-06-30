// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    public class BufferResource : PublicResource
    {
        [ReferenceOverride(nameof(Children), AutoSave = true)]
        public IReferences<BufferValue> Values { get; set; }

        [EditorBrowsable, DisplayName("Add Value")]
        [Description("Add typed value to the buffer")]
        public int AddValue([ResourceTypes(typeof(BufferValue))]string type, string name, string value)
        {
            var bufferValue = Graph.Instantiate<BufferValue>(type);
            bufferValue.Name = name;
            bufferValue.Value = value;
            Values.Add(bufferValue);

            return Values.Count;
        }

        [EditorBrowsable, DisplayName("Remove Value")]
        public int RemoveValue(string name)
        {
            var value = Values.FirstOrDefault(v => v.Name == name);
            if (value != null)
                Graph.Destroy(value, true);
            return Values.Count;
        }
    }
    
    public class BufferValue : Resource
    {
        private string _value;

        [DataMember, EditorBrowsable]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
    }

    public class ExtendedBufferValue : BufferValue
    {
    }
}
