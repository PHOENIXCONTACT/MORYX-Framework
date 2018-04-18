namespace Marvin.Resources.Management.Tests
{
    public class DerivedResource : SimpleResource
    {
        public override int MultiplyFoo(int factor)
        {
            return Foo *= factor + 1;
        }
    }
}