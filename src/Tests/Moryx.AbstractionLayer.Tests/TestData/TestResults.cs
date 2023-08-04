using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.Tests.TestData
{
    public enum TestResults
    {
        Success = 1,
        [Display(Name = "This is a failed result")]
        Failed = 2
    }
}
