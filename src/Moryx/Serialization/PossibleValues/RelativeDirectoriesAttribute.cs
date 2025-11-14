// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization
{
    /// <summary>
    /// <see cref="PossibleValuesAttribute"/> to provide a path relative to the BaseDirectory
    /// </summary>
    public class RelativeDirectoriesAttribute : PossibleValuesAttribute
    {
        private readonly string _parentPath;

        /// <summary>
        /// Creates a new instance of <see cref="RelativeDirectoriesAttribute"/>
        /// </summary>
        public RelativeDirectoriesAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RelativeDirectoriesAttribute"/>
        /// </summary>
        public RelativeDirectoriesAttribute(string parentPath)
        {
            _parentPath = parentPath;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var dirs = new List<string>();
            var childDirs = Directory.GetDirectories(currentDir);

            if (!string.IsNullOrEmpty(_parentPath) && childDirs.Contains(Path.Combine(currentDir, _parentPath)))
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

        /// <inheritdoc />
        public override bool OverridesConversion => false;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;

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

    }
}
