using System.Windows;
using System.Windows.Controls;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Interaction logic for ResourceMethodsControl.xaml
    /// </summary>
    public partial class ResourceMethodsControl : UserControl
    {
        /// <summary>
        /// Create methods control
        /// </summary>
        public ResourceMethodsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the methods
        /// </summary>
        public static readonly DependencyProperty MethodsProperty = DependencyProperty.Register(
            "Methods", typeof(ResourceMethodViewModel[]), typeof(ResourceMethodsControl), new PropertyMetadata(default(ResourceMethodViewModel[])));

        /// <summary>
        /// All methods that shall be displayed
        /// </summary>
        public ResourceMethodViewModel[] Methods
        {
            get { return (ResourceMethodViewModel[]) GetValue(MethodsProperty); }
            set { SetValue(MethodsProperty, value); }
        }
    }
}
