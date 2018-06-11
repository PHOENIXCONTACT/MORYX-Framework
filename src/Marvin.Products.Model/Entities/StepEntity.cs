using Marvin.Model;
using System.Collections.Generic;

namespace Marvin.Products.Model
{
    public class StepEntity : ModificationTrackedEntityBase
    {
        public virtual long StepId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Assembly { get; set; }

        public virtual string NameSpace { get; set; }

        public virtual string Classname { get; set; }

        public virtual string Parameters { get; set; }

        public virtual long WorkplanId { get; set; }

        public virtual long? SubWorkplanId { get; set; }

        public virtual WorkplanEntity Workplan { get; set; }

        public virtual WorkplanEntity SubWorkplan { get; set; }

        public virtual ICollection<ConnectorReference> Connectors { get; set; }

        public virtual ICollection<OutputDescriptionEntity> OutputDescriptions { get; set; }
    }
}
