using System.Collections.Generic;

namespace Marvin.Serialization
{
    /// <summary>
    /// Interface for all config entry collections
    /// </summary>
    public interface IEntryCollection
    {
        /// <summary>
        /// Export the current collection as list of config entries
        /// </summary>
        /// <returns></returns>
        List<Entry> ConfigEntries();
    }
}