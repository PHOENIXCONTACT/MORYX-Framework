using CommandLine;

namespace Marvin.Runtime.Kernel.DbUpdate
{
    /// <summary>
    /// Option class for the <see cref="DbUpdateRunMode"/>
    /// </summary>
    [Verb("dbUpdate", HelpText = "Updates all existing databases.")]
    public class DbUpdateOptions : RuntimeOptions
    {
    }
}