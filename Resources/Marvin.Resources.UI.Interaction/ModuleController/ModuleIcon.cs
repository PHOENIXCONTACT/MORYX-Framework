using System.Windows.Media;
using C4I;
using Marvin.ClientFramework;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Icon for the <see cref="ModuleController"/>
    /// </summary>
    [ModuleIconRegistration]
    public class ModuleIcon : IModuleIcon
    {
        ///
        public Geometry Icon
        {
            get { return ShapeFactory.GetShapeGeometry(CommonShapeType.Cells); }
        }
    }
}