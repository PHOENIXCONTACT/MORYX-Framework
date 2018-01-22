using System.Windows;
using System.Windows.Controls;

namespace Marvin.Resources.UI.Interaction
{
    public class ReferenceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SingleReference { get; set; }

        public DataTemplate MultiReference { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MultiReferenceViewModel)
                return MultiReference;

            return SingleReference;
        }
    }
}