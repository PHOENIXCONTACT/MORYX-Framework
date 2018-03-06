using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class TestConfig3
    {
        [DataMember]
        public List<TestConfig1> Configs { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }
    }
}
