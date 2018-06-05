using Marvin.Model;

namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Response object for testing database connections
    /// </summary>
    public class TestConnectionResponse
    {
        /// <summary>
        /// Result of the connection test
        /// </summary>
        public TestConnectionResult Result { get; set; }
    }
}
