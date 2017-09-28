using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marvin.Model;

namespace $safeprojectname$
{
    internal class FirstUpdate : IDatabaseUpdate
    {
        /// <summary>
        /// Description of this setup script
        /// </summary>
        public string Description { get { return "Column 'NewColumn' added."; } }

        /// <summary>
        /// Version from which the database should be migrated
        /// </summary>
        public int From { get { return 1; }}

        /// <summary>
        /// Version to which the database should be migrated
        /// </summary>
        public int To { get { return 2; } }

        /// <summary>
        /// Executes the update 
        /// </summary>
        public void Update(IUpdateContext context)
        {
            context.ExecuteScript("FirstUpdate.sql");
        }
    }
}
