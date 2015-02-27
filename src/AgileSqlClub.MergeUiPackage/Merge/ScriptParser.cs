using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.DocumentStructures;
using AgileSqlClub.MergeUi.Merge.ScriptDom;
using AgileSqlClub.MergeUi.Metadata;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Merge
{
    class ScriptParser
    {
        private readonly string _path;
        private readonly VsProject _project;

        public ScriptParser(string path, VsProject project)
        {
            _path = path;
            _project = project;
        }

        public List<ITable> GetDataTables()
        {
            var tables = new List<ITable>();

            using (var reader = new StreamReader(_path))
            {
                var parser = new TSql120Parser(true);

                IList<ParseError> errors;
                TSqlFragment sqlFragment = parser.Parse(reader, out errors);

                var visitor = new MergeVisitor();
                sqlFragment.Accept(visitor);

                foreach (var merge in visitor.Merges)
                {
                    tables.Add(new MergeStatementParser(merge).GetDescriptor(_path, _project));
                }
            }

            return tables;
        }
    }
}
