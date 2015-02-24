using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Merge.ScriptDom
{
    public class MergeVisitor : TSqlFragmentVisitor
    {
        public MergeVisitor()
        {
            Merges = new List<MergeStatement>();
        }

        public List<MergeStatement> Merges { private set; get; }

        public override void ExplicitVisit(MergeStatement node)
        {
            Merges.Add(node);
            base.ExplicitVisit(node);
        }
    }
}
