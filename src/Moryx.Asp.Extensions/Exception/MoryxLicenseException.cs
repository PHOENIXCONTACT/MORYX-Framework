using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WupiEngine;

namespace Moryx.Asp.Extensions
{
    /// <summary>
    /// License missing exception class
    /// </summary>
    public class MoryxLicenseException : WupiException
    {
        public MoryxLicenseException(WupiException wxcpt) : base(wxcpt)
        {
        }

    }
}
