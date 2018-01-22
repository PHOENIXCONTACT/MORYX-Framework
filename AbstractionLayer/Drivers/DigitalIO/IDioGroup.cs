namespace Marvin.AbstractionLayer.Drivers.DigitalIO
{
    /// <summary>
    /// Interface for a digital IO group
    /// </summary>
    public interface IDioGroup
    {
        /// <summary>
        /// Read or write the binary value for a group at a given starting adress
        /// </summary>
        /// <param name="startAdress"></param>
        /// <returns></returns>
        byte this[int startAdress] { get; set; }

        /// <summary>
        /// Read or write the binary value of the group
        /// </summary>
        byte this[string name] { get; set; }
    }
}