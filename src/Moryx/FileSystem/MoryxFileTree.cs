using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.FileSystem
{
    public class MoryxFileTree : MoryxFile
    {
        private List<MoryxFile> _files;

        public override FileType FileType => FileType.Tree;

        public IReadOnlyList<MoryxFile> Files => _files;

        public MoryxFileTree(List<MoryxFile> files)
        {
            _files = files;
        }
    }
}
