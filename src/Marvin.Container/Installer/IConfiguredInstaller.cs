namespace Marvin.Container
{
    internal interface IConfiguredInstaller
    {
        /// <summary>
        /// Set config on installer
        /// </summary>
        void SetRegistrator(ComponentRegistrator config);
    }
}
