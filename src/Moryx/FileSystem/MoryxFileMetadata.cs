using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moryx.FileSystem
{
    public class MoryxFileMetadata
    {
        public int Mode { get; set; } = 100644;

        public FileType FileType { get; set; }

        public string MimeType { get; set; }

        public string Hash { get; set; }

        public string FileName { get; set; }

        public string ToLine()
        {
            return $"{Mode} {FileType.ToString().ToLower()} {MimeType} {Hash} {FileName}";
        }

        public static MoryxFileMetadata FromLine(string line)
        {
            var parts = line.Split(' ');
            return new MoryxFileMetadata
            {
                Mode = int.Parse(parts[0]),
                FileType = (FileType)Enum.Parse(typeof(FileType), parts[1]),
                MimeType = parts[2],
                Hash = parts[3],
                FileName = string.Join(" ", parts.Skip(4))
            };
        }
    }

    /// <summary>
    /// Moryx file types in owner tree file
    /// </summary>
    public enum FileType
    {
        Blob = 0,

        Tree = 1,
    }
}
