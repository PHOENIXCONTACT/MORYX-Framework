using System.IO;
using System.Text;

namespace Marvin.Tests
{
    public class MemoryStreamDummy
    {
        public MemoryStreamDummy(string testString)
        {
            MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
        }

        public MemoryStream MemoryStream { get; set; }
    }
}
