using System.Collections.Generic;
using System.Data;
using AgileSqlClub.MergeUi.Merge;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface ITable
    {
        string SchemaName { get; set; }
        string Name { get; set; }
        List<ColumnDescriptor> Columns { get; set; }
        List<string> KeyColumns { get; set; }

        DataTable Data { get; set; }
    }

    public class Table : ITable
    {
        public string SchemaName { get; set; }

        public string Name { get; set; }

        public List<ColumnDescriptor> Columns { get; set; }

        public List<string> KeyColumns { get; set; }

        public DataTable Data { get; set; }
    }

}