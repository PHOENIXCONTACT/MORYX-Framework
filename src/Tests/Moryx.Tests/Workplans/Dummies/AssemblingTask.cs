using Moryx.AbstractionLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moryx.Tests.Workplans.Dummies
{
    [Display(Name = "Assembling Task", Description = "Task which does something with a product")]
    public class AssemblingTask : TaskStep<AssemblingActivity, AssemblingParameters>
    {
    }
}
