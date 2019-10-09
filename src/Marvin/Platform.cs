using System;

namespace Marvin
{
    /// <summary>
    /// This class provides access to the product this application belongs to and the platform it was build on.
    /// </summary>
    public abstract class Platform
    {
        /// <summary>
        /// Platform the current application was build on.
        /// </summary>
        public static Platform Current { get; protected set; }

        /// <summary>
        /// Type of this platform characterized with enum flags
        /// </summary>
        public abstract PlatformType Type { get; }

        /// <summary>
        /// Name of this platform
        /// </summary>
        public string PlatformName { get; protected set; }

        /// <summary>
        /// Version of the platform
        /// </summary>
        public Version PlatformVersion { get; protected set; }

        /// <summary>
        /// Name of the product this application belongs to
        /// </summary>
        public string ProductName { get; protected set; }

        /// <summary>
        /// Current version of this product
        /// </summary>
        public Version ProductVersion { get; protected set; }

        /// <summary>
        /// Description of the MARVIN based product
        /// </summary>
        public string ProductDescription { get; protected set; }
    }

    /// <summary>
    /// The different Marvin based platforms
    /// Flags: Server Client Embedded Mobile
    /// </summary>
    [Flags]
    public enum PlatformType
    {
        /// <summary>
        /// Platform provides server functionality
        /// </summary>
        Server = 0x8,
        /// <summary>
        /// Platform is a client
        /// </summary>
        Client = 0x4,
        /// <summary>
        /// Platform runs on small embedded devices
        /// </summary>
        Embedded = 0x2,
        /// <summary>
        /// Platform is optimized for mobile devices
        /// </summary>
        Mobile = 0x1,
    }
}
