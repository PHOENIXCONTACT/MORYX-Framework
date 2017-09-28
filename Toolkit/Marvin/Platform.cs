using System;

namespace Marvin
{
    /// <summary>
    /// This class provides access to the product this application belongs to and the platform it was build on.
    /// </summary>
    public abstract class Platform
    {
        /// <summary>
        /// Reference to be filled be inherited platform to provide unique API
        /// </summary>
        protected static Platform CurrentPlatform { get; set; }
        /// <summary>
        /// Platform the current application was build on.
        /// </summary>
        public static Platform Current { get { return CurrentPlatform; } }

        /// <summary>
        /// Type of this platform characterized with enum flags
        /// </summary>
        public abstract PlatformType Type { get; }
        /// <summary>
        /// Name of this platform
        /// </summary>
        public abstract string PlatformName { get; }
        /// <summary>
        /// Version of the platform
        /// </summary>
        public abstract Version PlatformVersion { get; }

        /// <summary>
        /// Name of the product this application belongs to
        /// </summary>
        public abstract string ProductName { get; }
        /// <summary>
        /// Current version of this product
        /// </summary>
        public abstract Version ProductVersion { get; }
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
