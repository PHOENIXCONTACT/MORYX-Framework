using System;
using System.Windows.Forms;

namespace DataModelWizard
{
    public partial class InputForm : Form
    {
        private const string ModelDocumentation = "http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-Daily-Doxygen/Documentation/runtime-dataModels.html";

        private const string WizardHowTo = "http://nts-eu-jenk02.europe.phoenixcontact.com:8080/job/MarvinPlatform-Daily-Doxygen/Documentation/CreateAndCleaningProject.html";

        public InputForm()
        {
            InitializeComponent();

            var toolTip1 = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 300,
                ReshowDelay = 200,
                ShowAlways = true
            };

            toolTip1.SetToolTip(OKButton, "Close the wizard and create the project.");

            var ttModelName = "The name of your Model. Likely something like 'MyProducts'.";
            toolTip1.SetToolTip(_modelName, ttModelName);
            toolTip1.SetToolTip(label2, ttModelName);

            var ttServerType = "The type of server-type that will wil host your database.";
            toolTip1.SetToolTip(_comboBox1, ttServerType);
            toolTip1.SetToolTip(label6, ttServerType);

            var ttUsesInheritance = "Check this box if your model shall be inherited from a base model.";
            toolTip1.SetToolTip(_usesInheritance, ttUsesInheritance);
            toolTip1.SetToolTip(label3, ttUsesInheritance);

            var ttBaseModel = "The name of your base model from which your model inherits properties. Could e.g. be something like 'Products' or 'Resources' from the AbstractionLayer-Solution.";
            toolTip1.SetToolTip(_baseModel, ttBaseModel);
            toolTip1.SetToolTip(label4, ttBaseModel);

            var ttBaseNameSpace = "The namespace of your base model from which your model inherits properties. Could e.g. be something like 'Marvin.Products.Model' or 'Marvin.Resources.Model'.";
            toolTip1.SetToolTip(_baseNamespace, ttBaseNameSpace);
            toolTip1.SetToolTip(label5, ttBaseNameSpace);

            toolTip1.SetToolTip(_docuLink, "Model documentation: " + ModelDocumentation);
            toolTip1.SetToolTip(_wizardDocu, "Wizard documentation: " + WizardHowTo);

            
        }

        private ModelConfiguration _config;
        public ModelConfiguration Configuration => _config;

        private ModelConfiguration BuildConfig()
        {
            // Copy all input values
            var config = new ModelConfiguration
            {
                ModelName = _modelName.Text,
                UsesInheritance = _usesInheritance.Checked,
                InheritedModel = _baseModel.Text,
                InheritedNamespace = _baseNamespace.Text,
                ServerType = _comboBox1.Text
            };

            return config;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            _config = BuildConfig();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _baseModel_TextChanged(object sender, EventArgs e)
        {
            _baseNamespace.Text = $@"Marvin.{((TextBox) sender).Text}.Model";
        }

        private void _docuLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(ModelDocumentation);
        }

        private void _wizardDocu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(WizardHowTo);
        }
    }
}
