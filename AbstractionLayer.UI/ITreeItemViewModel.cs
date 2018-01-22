namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Base interface for tree items. Will help to handle <see cref="IsExpanded"/> and <see cref="IsSelected"/>
    /// behavior and merge trees
    /// </summary>
    public interface ITreeItemViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this item is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        bool IsSelected { get; set; }
    }
}
