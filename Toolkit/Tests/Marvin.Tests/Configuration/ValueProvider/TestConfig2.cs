using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class TestConfig2
    {
        [DataMember]
        public TestConfig1 Config { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }
    }
}
