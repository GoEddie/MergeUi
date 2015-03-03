using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using AgileSqlClub.MergeUi.PackagePlumbing;
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
            bool failure = false;
            bool multipleFailures = false;
            using (var reader = new StreamReader(_path))
            {
                var parser = new TSql120Parser(true);

                IList<ParseError> errors;
                TSqlFragment sqlFragment = parser.Parse(reader, out errors);

                var visitor = new MergeVisitor();
                sqlFragment.Accept(visitor);

                foreach (var merge in visitor.Merges)
                {
                    try
                    {
                        tables.Add(new MergeStatementParser(merge).GetDescriptor(_path, _project));
                    }
                    catch (MergeStatamentParsingException msp)
                    {
                        OutputWindowMessage.WriteMessage("Unable to read table from the script file: {0} - error message:", _path);
                        OutputWindowMessage.WriteMessage(msp.Message);
                        if (failure)
                            multipleFailures = true;

                        failure = true;

                        
                    }
                }
            }

            if (failure)
            {
                var message = multipleFailures
                    ? "Unable to read some tables from the post-deploy script.\r\nCheck the output window for messages, correct and refresh\r\nChanges to those tables will not be saved"
                    : "Unable to read a table from the post-deploy script.\r\nCheck the output window for messages, correct and refresh\r\nChanges to that table will not be saved";

                MessageBox.Show(message);
            }

            return tables;
        }
    }
}
