using System.Collections.Generic;
using System.Windows.Forms;
using DataModelWizard;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace Marvin.Templates.ModelWizard
{
    public class DataModelWizard : IWizard
    {
        private ModelConfiguration _serviceConfiguration;

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param><param name="replacementsDictionary">The list of standard parameters to be replaced.</param><param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param><param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            var form = new InputForm();

            if (form.ShowDialog() != DialogResult.OK)
                throw new WizardCancelledException ("The user cancelled the wizard");

            _serviceConfiguration = form.Configuration;

            replacementsDictionary["$modelname$"] = _serviceConfiguration.ModelName;
            replacementsDictionary["$inherited$"] = _serviceConfiguration.UsesInheritance ? "true" : "false";
            replacementsDictionary["$inheritedmodel$"] = _serviceConfiguration.InheritedModel;
            replacementsDictionary["$inheritednamespace$"] = _serviceConfiguration.InheritedNamespace;
            replacementsDictionary["$server$"] = _serviceConfiguration.ServerType;

            if (_serviceConfiguration.ServerType == "PostgreSQL")
            {
                replacementsDictionary["$provider$"]      = "Devart.Data.PostgreSql";
                replacementsDictionary["$serverversion$"] = "9.2";
            }
            else if (_serviceConfiguration.ServerType == "SQL Server")
            {
                replacementsDictionary["$provider$"]      = "System.Data.SqlClient";
                replacementsDictionary["$serverversion$"] = "2012";
            }
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <returns>
        /// true if the project item should be added to the project; otherwise, false.
        /// </returns>
        /// <param name="filePath">The path to the project item.</param>
        public bool ShouldAddProjectItem(string filePath)
        {
            if (filePath.Contains("Inheritance") && !_serviceConfiguration.UsesInheritance)
                return false;
            return true;
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
        }

        /// <summary>
        /// Runs custom wizard logic before opening an item in the template.
        /// </summary>
        /// <param name="projectItem">The project item that will be opened.</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public void ProjectFinishedGenerating(Project project)
        {
        }
    }
}
