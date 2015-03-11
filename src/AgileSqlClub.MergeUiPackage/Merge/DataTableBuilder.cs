using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUI.Extensions;

namespace AgileSqlClub.MergeUI.Merge
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

            _table.ExtendedProperties.Add(DataTablePropertyNames.DataChanged, false);
            _table.EnableDirtyWatcher();
        }

        public DataTable Get()
        {
            return _table;
        }

    }
}
