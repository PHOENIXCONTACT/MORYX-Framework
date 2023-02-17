namespace Moryx.Runtime.Endpoints
{
    public static class RuntimePermissions
    {
        private const string _prefix = "Moryx.Runtime.";
        private const string _databasePrefix = _prefix + "Database.";
        private const string _commonPrefix = _prefix + "Common.";
        public const string DatabaseCanView = _databasePrefix + "CanView";
        public const string DatabaseCanSetAndTestConfig = _databasePrefix + "CanSetAndTestConfig";
        public const string DatabaseCanCreate = _databasePrefix + "CanCreate";
        public const string DatabaseCanErase = _databasePrefix + "CanErase";
        public const string DatabaseCanDumpAndRestore = _databasePrefix + "CanDumpAndRestore";
        public const string DatabaseCanMigrateModel = _databasePrefix + "CanMigrateModel";
        public const string DatabaseCanSetup = _databasePrefix + "CanSetup";
        public const string CanGetGeneralInformation = _commonPrefix + "CanGetGeneralInformation";
    }
}
