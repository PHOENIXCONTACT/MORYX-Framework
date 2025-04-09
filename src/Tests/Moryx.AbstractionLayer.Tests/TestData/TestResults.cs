using System.ComponentModel.DataAnnotations;

namespace Moryx.AbstractionLayer.Tests.TestData
{
    public enum TestResults
    {
        Success = 1,
        [Display(Name = "This is a failed result")]
        Failed = 2
    }
}
