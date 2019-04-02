using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Serialization;

namespace Marvin.Tests
{
    [ClassDisplay(Name = nameof(strings.ClassName), ResourceType = typeof(strings))]
    public class LocalizedClass
    {
        public const string PropDisplayNameAttributeDisplayName = "Display Name";

        [Display(Name = nameof(strings.PropDisplayAttribute_Name), Description = nameof(strings.PropDisplayAttribute_Description), ResourceType = typeof(strings))]
        public string PropDisplayAttribute { get; set; }

        [DisplayName(PropDisplayNameAttributeDisplayName)]
        public string PropDisplayNameAttribute { get; set; }
    }

    public class LocalizedMethodDummyClass
    {
        [Display(Name = nameof(strings.InitiateWorldTermination), Description = nameof(strings.InitiateWorldTerminationDescription), ResourceType = typeof(strings))]
        public bool InitiateWorldTermination([Display(Name = nameof(strings.EvacuatePeopleParam), Description = nameof(strings.EvacuatePeopleParamDescription), ResourceType = typeof(strings))] bool evacuatePeople,
            [Display(Name = nameof(strings.NameOfTerminatorParam), ResourceType = typeof(strings))] string nameOfTerminator)
        {
            return true;
        }
    }

    [ClassDisplay(Name = nameof(strings.CrashTestDummy), ResourceType = typeof(strings))]
    public class DummyClass
    {
        [Display(Name = nameof(strings.Number), Description = nameof(strings.NumberDescription), ResourceType = typeof(strings))]
        public int Number { get; set; }

        public string Name { get; set; }

        public int ReadOnly => 10;

        [Display(Name = nameof(strings.SubClass), Description = nameof(strings.SubClassDescription), ResourceType = typeof(strings))]
        public SubClass SingleClass { get; set; }

        public SubClass[] SubArray { get; set; }

        public List<SubClass> SubList { get; set; }

        public IEnumerable<SubClass> SubEnumerable { get; set; }

        public IDictionary<int, SubClass> SubDictionary { get; set; }

        public DummyEnum[] EnumArray { get; set; }

        public List<DummyEnum> EnumList { get; set; }

        public IEnumerable<DummyEnum> EnumEnumerable { get; set; }

        public bool[] BoolArray { get; set; }

        public List<bool> BoolList { get; set; }

        public IEnumerable<DummyEnum> BoolEnumerable { get; set; }

        [Display(Name = nameof(strings.SubClass), Description = nameof(strings.SubClassDescription))]
        public SubClass SingleClassNonLocalized { get; set; }
    }

    [ClassDisplay(Name = nameof(strings.NoShownAsProperty), Description = nameof(strings.NoShownAsPropertyDescription), ResourceType = typeof(strings))]
    public class SubClass
    {
        public float Foo { get; set; }

        public DummyEnum Enum { get; set; }

        public DummyEnum[] EnumArray { get; set; }
    }

    public enum DummyEnum
    {
        Unset,
        ValueA,
        ValueB
    }
}