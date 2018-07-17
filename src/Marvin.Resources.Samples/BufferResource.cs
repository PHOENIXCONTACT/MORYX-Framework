using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration]
    public class BufferResource : PublicResource
    {
        [ReferenceOverride(nameof(Children), AutoSave = true)]
        public IReferences<BufferValue> Values { get; set; }

        [EditorVisible, DisplayName("Add Value")]
        [Description("Add typed value to the buffer")]
        public int AddValue([ResourceTypes(typeof(BufferValue))]string type, string name, string value)
        {
            var bufferValue = Creator.Instantiate<BufferValue>(type);
            bufferValue.Name = name;
            bufferValue.Value = value;
            Values.Add(bufferValue);

            return Values.Count;
        }

        [EditorVisible, DisplayName("Remove Value")]
        public int RemoveValue(string name)
        {
            var value = Values.FirstOrDefault(v => v.Name == name);
            if (value != null)
                Creator.Destroy(value, true);
            return Values.Count;
        }
    }
    
    public class BufferValue : Resource
    {
        private string _value;

        [DataMember, EditorVisible]
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