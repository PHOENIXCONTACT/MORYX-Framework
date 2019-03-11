using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Properties;

namespace Marvin.Tests
{
    [ClassDisplay(Name = nameof(Localization.CrashTestDummy), ResourceType = typeof(Localization))]
    public class DummyClass
    {
        [Display(Name = nameof(Localization.Number), Description = nameof(Localization.NumberDescription), ResourceType = typeof(Localization))]
        public int Number { get; set; }

        public string Name { get; set; }

        public int ReadOnly => 10;

        [Display(Name = nameof(Localization.SubClass), Description = nameof(Localization.SubClassDescription))]
        public SubClass SingleClassNonLocalized { get; set; }

        [Display(Name = nameof(Localization.SubClass), Description = nameof(Localization.SubClassDescription), ResourceType = typeof(Localization))]
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
    }

    [ClassDisplay(Name = nameof(Localization.NoShownAsProperty), Description = nameof(Localization.NoShownAsPropertyDescription), ResourceType = typeof(Localization))]
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