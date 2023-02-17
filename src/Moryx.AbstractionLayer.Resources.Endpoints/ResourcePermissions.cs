namespace Moryx.AbstractionLayer.Resources.Endpoints
{
    public static class ResourcePermissions
    {
        private const string _prefix = "Moryx.Resources.";
        public const string CanViewTree = _prefix + "CanViewTree";
        public const string CanViewDetails = _prefix + "CanViewDetails";
        public const string CanEdit = _prefix + "CanEdit";
        public const string CanViewTypeTree = _prefix + "CanAddResource";
        public const string CanAdd = _prefix + "CanAdd";
        public const string CanDelete = _prefix + "CanDelete";
        public const string CanInvokeMethod = _prefix + "CanInvokeMethod";
    }
}
