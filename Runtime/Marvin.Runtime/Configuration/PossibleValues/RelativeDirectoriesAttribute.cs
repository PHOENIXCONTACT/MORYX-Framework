using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marvin.Container;

namespace Marvin.Runtime.Configuration
{
    public class RelativeDirectoriesAttribute : PossibleConfigValuesAttribute
    {
        private readonly string _parentPath;
        public RelativeDirectoriesAttribute()
        {
        }
        public RelativeDirectoriesAttribute(string parentPath)
        {
            _parentPath = parentPath;
        }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var dirs = new List<string>();
            var childDirs = Directory.GetDirectories(currentDir);

            if(!string.IsNullOrEmpty(_parentPath) && childDirs.Contains(Path.Combine(currentDir, _parentPath)))
            {
                currentDir = Path.Combine(currentDir, _parentPath);
                childDirs = Directory.GetDirectories(currentDir);
            }

            foreach (var directory in childDirs)
            {
                dirs.Add(directory.Replace(currentDir, ".\\"));
                dirs.AddRange(GetSubDirectories(directory).Select(dir => dir.Replace(currentDir, ".\\")));
            }
            return dirs;
        }

        private IEnumerable<string> GetSubDirectories(string path)
        {
            var dirs = new List<string>();
            foreach (var directory in Directory.GetDirectories(path))
            {
                dirs.Add(directory);
                dirs.AddRange(GetSubDirectories(directory));
            }
            return dirs;
        }

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public override bool OverridesConversion
        {
            get { return false; }
        }

        public override bool UpdateFromPredecessor
        {
            get { return false; }
        }
    }
}
