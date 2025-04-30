using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.FileSystem
{
    public class MoryxFileMode
    {
        private int _value = FileBase + 666;

        public static int FileBase = 100000;

        public static int Admin(int access) => access * 100;
        
        public static int Owner(int access) => access * 10;

        public static int Public(int access) => access;

        public static int Read = 4;

        public static int Write = 6;

        public static int Execute = 5;

        public static explicit operator int(MoryxFileMode mode) => mode._value;

        public static explicit operator MoryxFileMode(int value) => new MoryxFileMode { _value = value };
    }
}
