using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Main execution token passed trough the workflow
    /// </summary>
    internal class MainToken : IToken
    {
        /// <summary>
        /// Token name
        /// </summary>
        public string Name => "MainToken";
    }
}