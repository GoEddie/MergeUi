using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Metadata;

namespace AgileSqlClub.MergeUi.Merge
{
    public class MergeStatementDescriptor : ITable
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
        public List<ColumnDescriptor> Columns { get; set; }
        public List<string> KeyColumns { get; set; }
        public DataTable Data { get;  set; }
    }


    public class MergeStatementFromScriptDescriptor : ITable
    {
        public string SchemaName { get; set; }

        public string Name { get; set; }
        public List<ColumnDescriptor> Columns { get; set; }
        public List<string> KeyColumns { get; set; }
        public DataTable Data { get; set; }

        public string OriginalStatement { get; set; }
        public string OriginalFilename { get; set; }
    }

}
