using Moryx.Workplans;
using System.Collections.Generic;

namespace Moryx.Workflows.Implementation
{
    public class ComparingProperties
    {
        public IWorkplanStep Step { get; set;}
        public IWorkplanStep NewStep { get; set; }
        public Workplan Workplan { get; set; }
        public Workplan NewWorkplan { get; set; }
        public List<IWorkplanStep> IsChecked { get; set; }
        public List<IWorkplanStep> NeedToCheck { get; set; }
        public List<IWorkplanStep> NewNeedToCheck { get; set; }
    }
    
}

