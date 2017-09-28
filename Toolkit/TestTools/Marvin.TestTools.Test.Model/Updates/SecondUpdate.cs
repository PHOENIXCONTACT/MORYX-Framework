using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model.Updates
{
    internal class SecondUpdate : IDatabaseUpdate
    {
        public string Description
        {
            get { return "Column 'SuperNewColumn' added!"; }
        }

        public int From
        {
            get { return 2; }
        }

        public int To
        {
            get { return 3; }
        }

        public void Update(IUpdateContext context)
        {
            context.ExecuteScript("SecondUpdate.sql");

            using(var uow = context.OpenUnitOfWork())
            {
                var repo = uow.GetRepository<ITopParentRepository>();
                var parent = repo.Create(new Random().Next());
                uow.Save();
            }
        }
    }
}
