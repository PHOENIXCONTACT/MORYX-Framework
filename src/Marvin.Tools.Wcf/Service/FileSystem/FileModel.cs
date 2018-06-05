using System;
using System.Runtime.Serialization;

namespace Marvin.Tools.Wcf.FileSystem
{
    public enum EFileType
    {
        File,
        Directory
    }

    [DataContract]
    public class FileModel
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public EFileType Type { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public DateTime? CreationDate { get; set; }

        [DataMember]
        public string MimeType { get; set; }
    }
}
