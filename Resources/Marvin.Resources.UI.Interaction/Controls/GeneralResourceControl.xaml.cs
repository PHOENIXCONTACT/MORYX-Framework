using System.Windows;
using System.Windows.Controls;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Interaction logic for GeneralResourceInformation.xaml
    /// </summary>
    public partial class GeneralResourceControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralResourceControl"/> class.
        /// </summary>
        public GeneralResourceControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the <see cref="Resource"/>
        /// </summary>
        public static readonly DependencyProperty ResourceProperty = DependencyProperty.Register(
            "Resource", typeof (IResourceHead), typeof (GeneralResourceControl), new PropertyMetadata(default(IResourceHead)));

        /// <summary>
        /// The resource property
        /// </summary>
        public IResourceHead Resource
        {
            get { return (IResourceHead) GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="IsEditMode"/>
        /// </summary>
        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(
            "IsEditMode", typeof (bool), typeof (GeneralResourceControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Edit Mode Property 
        /// </summary>
        public bool IsEditMode
        {
            get { return (bool) GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }
    }
}
