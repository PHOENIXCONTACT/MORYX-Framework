namespace Marvin.Tests.Bindings
{
    public class SomeHiddenPropertyClass : SomeClass
    {
        public new SomeImplementation SomeObject
        {
            get { return (SomeImplementation)base.SomeObject; }
            set { base.SomeObject = value; }
        }
    }
}