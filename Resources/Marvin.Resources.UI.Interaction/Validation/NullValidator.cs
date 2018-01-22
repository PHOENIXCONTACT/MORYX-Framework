namespace Marvin.Resources.UI.Interaction
{
    public class NullValidator : IValidationRule
    {
        public static NullValidator Instance = new NullValidator();
        public string ErrorMessage
        {
            get { return string.Empty; }
        }
        public bool Validate(object value)
        {
            return true;
        }
    }
}