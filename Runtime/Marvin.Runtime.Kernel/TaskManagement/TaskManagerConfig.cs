using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Runtime.Kernel.TaskManagement
{
    /// <summary>
    /// Configuration for the task manager.
    /// </summary>
    [DataContract]
    public class TaskManagerConfig : ConfigBase
    {
        /// <summary>
        /// Configurates the amount of threads which can handle tasks.
        /// </summary>
        [DataMember]
        public int ThreadCount { get; set; }
    }
}