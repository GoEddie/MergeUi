using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileSqlClub.MergeUi.Merge
{
    public class DataTableBuilder
    {
        private readonly DataTable _table;

        public DataTableBuilder(string name, List<ColumnDescriptor> columns)
        {
            _table = new DataTable(name);

            foreach (var col in columns)
            {
                _table.Columns.Add(new DataColumn(col.Name.Value));
            }
        }

        public DataTable Get()
        {
            return _table;
        }

    }
}
