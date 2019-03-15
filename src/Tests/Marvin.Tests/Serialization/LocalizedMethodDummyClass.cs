using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Properties;

namespace Marvin.Tests
{
    public class LocalizedMethodDummyClass
    {
        [Display(Name = nameof(strings.InitiateWorldTermination), Description = nameof(strings.InitiateWorldTerminationDescription), ResourceType = typeof(strings))]
        public bool InitiateWorldTermination([Display(Name = nameof(strings.EvacuatePeopleParam), Description = nameof(strings.EvacuatePeopleParamDescription), ResourceType = typeof(strings))] bool evacuatePeople,
                                             [Display(Name = nameof(strings.NameOfTerminatorParam), ResourceType = typeof(strings))] string nameOfTerminator)
        {
            return true;
        }
    }
}
