using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Merge
{
    public class TableDescriptor
    {
        public string Name { get; private set; }
        public List<ColumnDescriptor> Columns { get; private set; }
        public List<string> KeyColumns { get; private set; }
    }
}