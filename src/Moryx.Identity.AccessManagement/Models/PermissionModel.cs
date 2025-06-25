using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// Maps to <see cref="Permission"/>.
    /// </summary>
    public class PermissionModel
    {
        /// <summary>
        /// Maps to <see cref="Permission.Id"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Maps to <see cref="Permission.Name"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Maps to <see cref="Permission.Roles"/>.
        /// </summary>
        public string[] Roles { get; set; }
    }
}