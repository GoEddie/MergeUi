using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileSqlClub.MergeUi.Merge
{
    public class MergeStatementDescriptor
    {
        public TableDescriptor TableDescriptor { get; private set; }
        public DataTable Data { get; private set; }
    }
}
