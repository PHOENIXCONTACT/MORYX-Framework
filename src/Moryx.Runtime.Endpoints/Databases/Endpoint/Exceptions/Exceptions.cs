namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public string[] Errors { get; }

        public BadRequestException(string[] errors) : base(string.Join("\n", errors))
        {
            Errors = errors;
        }
    }
}
