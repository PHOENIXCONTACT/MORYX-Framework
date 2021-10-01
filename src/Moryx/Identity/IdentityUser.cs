namespace Moryx.Identity
{
    /// <summary>
    /// Model of a user
    /// </summary>
    public class IdentityUser
    {
        /// <summary>
        /// Unique user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
    }
}