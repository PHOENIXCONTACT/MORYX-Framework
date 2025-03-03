using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.FileSystem
{
    public class MoryxFileTree : MoryxFile
    {
        private readonly List<MoryxFile> _files = new List<MoryxFile>();

        public override FileType FileType => FileType.Tree;

        public IReadOnlyList<MoryxFile> Files => _files;

        public void AddFile(MoryxFile file)
        {
            file.ParentTree = this;
            _files.Add(file);
        }

        public bool RemoveFile(MoryxFile file)
        {
            var found = _files.Remove(file);
            if (found)
                file.ParentTree = null;
            return found;
        }
    }
}
