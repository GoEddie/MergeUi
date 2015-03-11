using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileSqlClub.MergeUI.Metadata
{
    internal class VsSchema : ISchema
    {
        private readonly string _schemaName;
        private readonly Dictionary<string, ITable> _tables = new Dictionary<string, ITable>();

        public VsSchema(string schemaName, IList<ITable> tables)
        {
            _schemaName = schemaName;

            foreach (var table in tables)
            {
                if(table.Name != null)
                    _tables[table.Name] = table;
            }
            
        }

        public ITable GetTable(string name)
        {
            if (_tables.ContainsKey(name))
                return _tables[name];

            return null;
        }

        public List<string> GetTables()
        {
            return _tables.Keys.ToList();
        }

        public void AddTable(ITable table)
        {
            _tables[table.Name] = table;
        }

        public string GetName()
        {
            return _schemaName;
        }


        public void Save(string scriptFile)
        {
            foreach (var table in _tables.Values)
            {
                table.Save(scriptFile);
            }
        }
    }
}
