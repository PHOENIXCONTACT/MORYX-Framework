using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using C4I;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Interaction logic for InteractionWorkspaceView.xaml
    /// </summary>
    public partial class InteractionWorkspaceView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionWorkspaceView"/> class.
        /// </summary>
        public InteractionWorkspaceView()
        {
            InitializeComponent();
        }

        private void OnResourceTreeeMouseDown(object sender, MouseButtonEventArgs e)
        {
            var treeView = (EddieTreeView)sender;
            if (treeView.SelectedItem == null)
                return;

            var hitTestResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitTestResult.VisualHit is TreeViewItem)
                return;

            var item = treeView.ContainerFromItem(treeView.SelectedItem);
            item.IsSelected = false;
            treeView.Focus();
        }

    }

    /// <summary>
    /// Extensions for the TreeView
    /// TODO: Move to Platform
    /// </summary>
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Loads the current container for the given item
        /// </summary>
        public static TreeViewItem ContainerFromItem(this TreeView treeView, object item)
        {
            var container = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
            return container ?? ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
        }

        private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentGenerator, IEnumerable itemCollection, object item)
        {
            foreach (object curChildItem in itemCollection)
            {
                var parentContainer = (TreeViewItem)parentGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                    return null;

                var container = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (container != null)
                    return container;

                container = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
                if (container != null)
                    return container;
            }

            return null;
        }
    }
}
