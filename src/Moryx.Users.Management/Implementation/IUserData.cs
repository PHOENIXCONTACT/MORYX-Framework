namespace Moryx.Users.Management
{
    /// <summary>
    /// Information about the user which can be signed in at the machine
    /// </summary>
    internal interface IUserData : IPersistentObject
    {
        InternalUser User { get; }

        string Identifier { get; }

        bool SignedIn { get; set; }
    }
}