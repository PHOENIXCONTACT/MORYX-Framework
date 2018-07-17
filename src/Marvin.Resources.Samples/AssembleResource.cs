using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration]
    [DisplayName("Handarbeitsplatz"), Description("Handarbeitsplatz für manuele Arbeiten.")]
    public class AssembleResource : Cell
    {
        [DataMember]
        public AssembleConfig Config { get; set; }

        [ResourceReference(ResourceRelationType.Extension)]
        public AssembleFoo Setup { get; set; }

        [ResourceReference(ResourceRelationType.Extension)]
        public IVisualInstructor Instructor { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Setup == null)
            {
                Setup = Creator.Instantiate<AssembleFoo>();
                RaiseResourceChanged();
            }
        }

        [EditorVisible]
        public void ChangeNumber(string fooNumber)
        {
            Setup.ChangeNumber(fooNumber);
        }

        [EditorVisible]
        public class AssembleConfig
        {
            public string Name { get; set; }

            public int Number { get; set; }
        }
        
    }

    public class AssembleFoo : Resource
    {
        [DataMember]
        public string FooNumber { get; set; }

        public void ChangeNumber(string fooNumber)
        {
            FooNumber = fooNumber;
            RaiseResourceChanged();
        }
    }
}