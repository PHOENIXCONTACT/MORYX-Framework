// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Model for a setup.
    /// </summary>
    public class SetupModel
    {
        /// <summary>
        /// The full name of the model type.
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// For this data model unique setup id
        /// </summary>
        public int SortOrder { get; set; } 
   
        /// <summary>
        /// Display name of this setup.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short description what data this setup contains.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Filetype supported by this setup
        /// </summary>
        public string SupportedFileRegex { get; set; }

        /// <summary>
        /// Name of the file of this setup.
        /// </summary>
        public string SetupData { get; set; }

        internal SetupModel CopyWithFile(string file)
        {
            var copy = new SetupModel
            {
                Fullname = Fullname,
                SortOrder = SortOrder,
                Name = Name,
                Description = string.Format("{0}\nFile: {1}", Description, file),
                SetupData = file
            };
            return copy;
        }
    }
}
