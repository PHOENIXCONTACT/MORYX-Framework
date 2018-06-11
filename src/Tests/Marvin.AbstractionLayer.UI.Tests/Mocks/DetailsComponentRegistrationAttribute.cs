namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DetailsComponentRegistrationAttribute : DetailsRegistrationAttribute
    {
        public DetailsComponentRegistrationAttribute(string typeName) : base(typeName, typeof(IDetailsComponent))
        {
        }
    }
}