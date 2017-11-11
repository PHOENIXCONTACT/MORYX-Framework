using System;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// The thread context to be used by console applications or Windows services for the <see cref="BaseWcfClientFactory"/>
    /// </summary>
    public class SimpleThreadContext : IThreadContext
    {
        /// 
        public void Invoke(Action action)
        {
            action();
        }
    }
}