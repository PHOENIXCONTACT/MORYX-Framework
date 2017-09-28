using Common.Logging;

namespace Marvin.Logging
{
    internal static class LogFactory
    {
        public static ILog Create(string name)
        {
            try
            {
                return LogManager.GetLogger(name);
            }
            catch
            {
                return new DummyLog();
            }
        }
    }
}
