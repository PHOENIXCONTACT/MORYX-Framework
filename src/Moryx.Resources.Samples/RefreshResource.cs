// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    public class RefreshResource : Resource
    {
        [EntrySerialize]
        public string CurrentTime => DateTime.Now.ToLongTimeString();
    }
}