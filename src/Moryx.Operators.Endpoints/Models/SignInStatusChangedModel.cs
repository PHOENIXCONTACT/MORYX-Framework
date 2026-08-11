using Moryx.Operators.Attendances;

namespace Moryx.Operators.Endpoints.Models
{
    internal class SignInStatusChangedModel
    {
        public required SignInStatus Status { get; set; }
        public required ExtendedOperatorModel Operator { get; set; }
        public required AttendableResourceModel Assignable { get; set; }
    }
}