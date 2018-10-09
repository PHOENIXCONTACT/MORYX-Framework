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