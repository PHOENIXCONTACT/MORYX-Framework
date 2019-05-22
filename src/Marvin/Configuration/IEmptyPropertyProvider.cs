namespace Marvin.Configuration
{
    /// <summary>
    /// Empty property provider to pre-fill newly created objects
    /// </summary>
    public interface IEmptyPropertyProvider
    {
        /// <summary>
        /// Fills the object
        /// </summary>
        void FillEmpty(object obj);
    }
}