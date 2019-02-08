using System.IO;

namespace Marvin.Tests
{
    public class FileStreamDummy
    {
        public FileStreamDummy(string filePath, FileMode mode)
        {
            FileStream = new FileStream(filePath, mode);
        }

        public FileStream FileStream { get; set; }
    }
}
