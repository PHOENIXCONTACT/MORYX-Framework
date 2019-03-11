using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Properties;

namespace Marvin.Tests
{
    public class LocalizedMethodDummyClass
    {
        [Display(Name = nameof(Localization.InitiateWorldTermination), Description = nameof(Localization.InitiateWorldTerminationDescription), ResourceType = typeof(Localization))]
        public bool InitiateWorldTermination([Display(Name = nameof(Localization.EvacuatePeopleParam), Description = nameof(Localization.EvacuatePeopleParamDescription), ResourceType = typeof(Localization))] bool evacuatePeople,
                                             [Display(Name = nameof(Localization.NameOfTerminatorParam), ResourceType = typeof(Localization))] string nameOfTerminator)
        {
            return true;
        }
    }
}
