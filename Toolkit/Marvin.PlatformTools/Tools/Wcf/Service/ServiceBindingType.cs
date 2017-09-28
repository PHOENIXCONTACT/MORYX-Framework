namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Type of binding
    /// </summary>
    public enum ServiceBindingType
    {
        /// <summary>
        /// WebHttp binding used for standard http methods and web clients
        /// </summary>
        WebHttp,

        /// <summary>
        /// Soap http binding
        /// </summary>
        BasicHttp,

        /// <summary>
        /// Net tcp binding for fast, session bound duplex connections
        /// </summary>
        NetTcp
    }
}