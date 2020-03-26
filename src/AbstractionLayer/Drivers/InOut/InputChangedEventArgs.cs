// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Event args for changes to an input
    /// </summary>
    public class InputChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Type of access for the changed value
        /// </summary>
        public SupportedAccess InputAccess { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Text key of the input
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Input changed for single value
        /// </summary>
        public InputChangedEventArgs()
        {
            InputAccess = SupportedAccess.Single;
        }

        /// <summary>
        /// Input changed for numeric index
        /// </summary>
        public InputChangedEventArgs(int index)
        {
            Index = index;
            InputAccess = SupportedAccess.Index;
        }

        /// <summary>
        /// Input changed for text key
        /// </summary>
        public InputChangedEventArgs(string key)
        {
            Key = key;
            InputAccess = SupportedAccess.Key;
        }

        /// <summary>
        /// Input can be access via index or key
        /// </summary>
        public InputChangedEventArgs(int index, string key)
        {
            Index = index;
            Key = key;
            InputAccess = SupportedAccess.Index | SupportedAccess.Key;
        }
    }
}
