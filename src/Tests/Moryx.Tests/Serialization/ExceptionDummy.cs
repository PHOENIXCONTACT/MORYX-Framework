// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tests
{
    public class ExceptionDummy
    {
        public int ThrowsException
        {
            get { throw new Exception("BAM!"); }
        } 
    }
}
