namespace Moryx.ControlSystem.Jobs.Endpoints
{
    public static class JobPermissions
    {
        private const string _prefix = "Moryx.Jobs.";
        public const string CanView = _prefix + "CanView";
        public const string CanComplete = _prefix + "CanComplete";
        public const string CanAbort = _prefix + "CanAbort";
    }

}
