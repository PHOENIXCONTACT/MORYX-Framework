namespace Marvin.Resources.UI.Interaction
{
    public class StringValidationRule : IValidationRule
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public StringValidationRule(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public string ErrorMessage { get; private set; }

        public bool Validate(object value)
        {
            var str = (string) value;

            if (_minLength > 0)
            {
                if (string.IsNullOrEmpty(str))
                {
                    ErrorMessage = string.Format("The text can not be empty.");
                    return false;
                }
            }

            if (str.Length < _minLength)
            {
                ErrorMessage = string.Format("The text must be longer than {0} characters.", _minLength);
                return false;
            }

            if (_maxLength != 0 && str.Length > _maxLength)
            {
                ErrorMessage = string.Format("The text must be smaller than {0} characters.", _maxLength);
                return false;
            }
            
            return true;
        }

    }
}