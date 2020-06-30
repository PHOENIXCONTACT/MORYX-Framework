// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Token representing a split execution
    /// </summary>
    internal class SplitToken : IToken
    {
        ///
        public string Name => $"{Original.Name}-Partial";

        public IToken Original { get; private set; }

        public SplitToken(IToken original)
        {
            Original = original;
        }
    }
}
