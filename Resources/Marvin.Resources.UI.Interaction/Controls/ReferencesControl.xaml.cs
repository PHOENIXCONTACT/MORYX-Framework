using System.Collections.ObjectModel;
using System.Windows;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Interaction logic for DriverReferencesControl.xaml
    /// </summary>
    public partial class ReferencesControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReferencesControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// List of DriveReferences that can be manipulated by this usercontrol
        /// </summary>
        public static readonly DependencyProperty ReferencesProperty = DependencyProperty.Register(
           "References", typeof(ReferenceViewModel[]), typeof(ReferencesControl),
           new PropertyMetadata(null));

        /// <summary>
        /// List of DriveReferences-Property
        /// </summary>
        public ReferenceViewModel[] References
        {
            get { return (ReferenceViewModel[])GetValue(ReferencesProperty); }
            set { SetValue(ReferencesProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="IsEditMode"/>
        /// </summary>
        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(
            "IsEditMode", typeof(bool), typeof(ReferencesControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Edit Mode Property 
        /// </summary>
        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }
    }
}
