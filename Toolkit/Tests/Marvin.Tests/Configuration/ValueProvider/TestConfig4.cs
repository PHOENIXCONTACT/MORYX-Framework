using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class TestConfig4
    {
        [DataMember]
        public List<int> Numbers { get; set; }

        [DataMember]
        public List<string> Strings { get; set; }

        [DataMember]
        public int[] ArrayNumbers { get; set; }

        [DataMember]
        public IEnumerable<int> EnumerableNumbers { get; set; }
    }
}
