using System.Diagnostics;

namespace Marvin.Runtime.Base
{
    internal class Caller
    {
        /// <summary>
        /// Class calling this method
        /// </summary>
        public string CallingClass { get; set; }
        /// <summary>
        /// Get calling method
        /// </summary>
        public string CallingMethod { get; set; }
    }

    internal static class CallerId
    {
        /// <summary>
        /// Get class und method calling this method
        /// </summary>
        /// <returns></returns>
        public static Caller GetCaller()
        {
            var stackFrame = new StackTrace().GetFrame(2);
            var method = stackFrame.GetMethod();
            var caller = new Caller()
            {
                CallingMethod = method.Name,
                CallingClass = method.DeclaringType != null ? method.DeclaringType.Name : "Anonymous type"
            };
            return caller;
        }
    }
}
