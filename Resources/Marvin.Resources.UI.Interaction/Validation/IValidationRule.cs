namespace Marvin.Resources.UI.Interaction
{
    public interface IValidationRule
    {
        string ErrorMessage { get; }

        bool Validate(object value);
    }
}