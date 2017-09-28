using System.Collections.Generic;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionTests
    {
        private readonly IEnumerable<int> _enumerable = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9
        };
        
        [Test]
        public void ForEachTest()
        {
            var enumerated = new List<int>();

            // ReSharper disable once ConvertClosureToMethodGroup
            _enumerable.ForEach(delegate(int i)
            {
                enumerated.Add(i);
            });

            foreach (var item in _enumerable)
            {
                Assert.IsTrue(enumerated.Contains(item));
            }
        }
    }
}