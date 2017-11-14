using Marvin.Workflows;

namespace Marvin.Tests.Workflows
{
    public class DummyToken : IToken
    {
        /// <summary>
        /// Token name
        /// </summary>
        public string Name { get { return "DummyToken"; } }
    }
}