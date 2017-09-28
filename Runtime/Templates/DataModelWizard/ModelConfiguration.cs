namespace DataModelWizard
{
    public class ModelConfiguration
    {
        public string ModelName { get; set; }
        public bool UsesInheritance { get; set; }
        public string InheritedModel { get; set; }
        public string InheritedNamespace { get; set; }
        public string ServerType { get; set; }
    }
}
