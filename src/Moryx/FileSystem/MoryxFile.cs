using System;
using System.Linq;

namespace Moryx.FileSystem
{
    public class MoryxFile
    {
        public MoryxFileMode Mode { get; set; } = new MoryxFileMode();

        public virtual FileType FileType { get; } = FileType.Blob;

        public string MimeType { get; set; }

        public string Hash { get; set; }

        public string FileName { get; set; }

        public MoryxFileTree ParentTree { get; set; }
    }
}
