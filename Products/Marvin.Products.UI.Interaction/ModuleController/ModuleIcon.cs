using System.Windows.Media;
using Marvin.ClientFramework;
namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Icon of the module
    /// </summary>
    [ModuleIconRegistration]
    public class ModuleIcon : IModuleIcon
    {
        /// <summary>
        /// Geometry for this module
        /// </summary>
        public static Geometry IconPath = Geometry.Parse(
            "M43.377,21.458c0.704,0.694,0.704,1.836,0,2.54L32.26,35.103c-0.697,0.704-1.841,0.704-2.53,0.021" +
            "c-0.707-0.728-0.702-1.856-0.01-2.561l11.123-11.104C41.544,20.754,42.673,20.754,43.377,21.458z M48.659,54.039" +
            "c0.704,0.704,1.843,0.704,2.537,0.011l11.117-11.123c0.709-0.704,0.704-1.844,0-2.542c-0.694-0.697-1.838-0.697-2.543,0" +
            "L48.659,51.496C47.954,52.206,47.954,53.345,48.659,54.039z M75.306,21.836v53.087H17.598v-0.855l-5.116,5.469L4.23,71.271" +
            "l13.367-12.5V0h36.685L75.306,21.836z M55.261,20.06h9.906L55.261,9.776V20.06z M69.236,24.614H50.709V6.077h-27.04v47.009" +
            "l13.569-12.691l-4.073-4.067l11.423-11.434L58.87,39.186l-11.413,11.42l-4.091-4.096l-19.697,21.06v1.279h45.567V24.614z");
        

        /// <summary>
        /// Will provide the geometry of the module icon
        /// </summary>
        public Geometry Icon
        {
            get { return IconPath; }
        }
    }
}