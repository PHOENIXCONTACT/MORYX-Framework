namespace Moryx.AbstractionLayer.Products.Endpoints
{
    public static class ProductPermissions
    {
        private const string _prefix = "Moryx.Products.";
        public const string CanViewTypes = _prefix + "CanViewTypes";
        public const string CanCreateAndEditRecipes = _prefix + "CanCreateAndEditRecipes";
        public const string CanEditType = _prefix + "CanEditType";
        public const string CanDuplicateType = _prefix + "CanDuplicateType";
        public const string CanImport = _prefix + "CanImport";
        public const string CanDeleteType = _prefix + "CanDeleteType";
        public const string CanViewInstances = _prefix + "CanViewInstances";
        public const string CanCreateInstances = _prefix + "CanCreateInstances";
    }
}
