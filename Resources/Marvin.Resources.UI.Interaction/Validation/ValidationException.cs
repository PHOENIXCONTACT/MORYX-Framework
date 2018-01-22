using System;

namespace Marvin.Resources.UI.Interaction
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
            
        }
    }
}