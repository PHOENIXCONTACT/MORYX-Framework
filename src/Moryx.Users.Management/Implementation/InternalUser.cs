using System.Diagnostics;

namespace Moryx.Users.Management
{
    internal class InternalUser : User
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Identifier
        {
            get => base.Identifier;
            set => base.Identifier = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string FirstName
        {
            get => base.FirstName;
            set => base.FirstName = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string LastName
        {
            get => base.LastName;
            set => base.LastName = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new bool SignedIn
        {
            get => base.SignedIn;
            set => base.SignedIn = value;
        }
    }
}