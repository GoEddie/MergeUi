using System.Collections.Generic;
using System.Data;
using AgileSqlClub.MergeUi.Merge;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface ITable
    {
        string SchemaName { get; set; }
        string Name { get; set; }
        List<ColumnDescriptor> Columns { get; set; }
        List<string> KeyColumns { get; set; }

        DataTable Data { get; set; }
        Merge Merge { get; set; }
        
    }

    public class Merge
    {
        public MergeStatement MergeStatement { get; set; }
        public string File { get; set; }
        public int ScriptOffset { get; set; }
        public int ScriptLength { get; set; }
    }


    public class Table : ITable
    {
        public Table()
        {
            Merge = new Merge();

        }

        public Table(string name, List<ColumnDescriptor> columns)
        {
            Merge = new Merge();
            Name = name;
            Columns = columns;
            Data = new DataTableBuilder(Name, Columns).Get();
        }

        public string SchemaName { get; set; }

        public string Name { get; set; }

        public List<ColumnDescriptor> Columns { get; set; }

        public List<string> KeyColumns { get; set; }

        public DataTable Data { get; set; }

        public Merge Merge { get; set; }
    }

}