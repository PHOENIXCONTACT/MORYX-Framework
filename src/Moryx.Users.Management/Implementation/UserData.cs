namespace Moryx.Users.Management
{
    internal class UserData : IUserData
    {
        public InternalUser User { get; }

        long IPersistentObject.Id { get; set; }

        public string Identifier
        {
            get => User.Identifier;
            set => User.Identifier = value;
        }

        public bool SignedIn
        {
            get => User.SignedIn;
            set => User.SignedIn = value;
        }

        public UserData()
        {
            User = new InternalUser();
        }
    }
}