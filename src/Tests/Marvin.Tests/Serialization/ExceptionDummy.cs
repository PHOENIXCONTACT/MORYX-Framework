// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Tests
{
    public class ExceptionDummy
    {
        public int ThrowsException
        {
            get { throw new Exception("BAM!"); }
        } 
    }
}
