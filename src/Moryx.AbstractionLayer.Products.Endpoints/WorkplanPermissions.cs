namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Permissions used to authorize for the <see cref="WorkplanController"/>
    /// </summary>
    public static class WorkplanPermissions
    {
        /// <summary>
        /// Prefix used for all permissions of the controller
        /// </summary>
        private const string _prefix = "Moryx.Workplans.";

        /// <summary>
        /// Permission for all actions related to viewing one or multiple workplans
        /// </summary>
        public const string CanView = _prefix + "CanView";

        /// <summary>
        /// Permission for all actions related to editing or creating one or multiple workplans
        /// </summary>
        public const string CanEdit = _prefix + "CanEdit";

        /// <summary>
        /// Permission for all actions related to deleting one or multiple workplans
        /// </summary>
        public const string CanDelete = _prefix + "CanDelete";
    }
}
