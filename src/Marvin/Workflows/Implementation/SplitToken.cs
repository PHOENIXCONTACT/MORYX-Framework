using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Token representing a split execution
    /// </summary>
    internal class SplitToken : IToken
    {
        ///
        public string Name => $"{Original.Name}-Partial";

        public IToken Original { get; private set; }

        public SplitToken(IToken original)
        {
            Original = original;
        }
    }
}