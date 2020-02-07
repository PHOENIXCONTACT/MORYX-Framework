// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Configuration of the backup.
    /// </summary>
    public class BackupModel
    {
        /// <summary>
        /// Name of the backup file.
        /// </summary>
        public string FileName { get; set; }
   
        /// <summary>
        /// Size of the current backup file.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Date of the backup creation.
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
