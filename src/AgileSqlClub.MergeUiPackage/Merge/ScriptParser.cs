using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using AgileSqlClub.MergeUI.PackagePlumbing;
using AgileSqlClub.MergeUI.Merge.ScriptDom;
using AgileSqlClub.MergeUI.Metadata;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using AgileSqlClub.MergeUI.UI;

namespace AgileSqlClub.MergeUI.Merge
{
    class ScriptParser
    {
        private readonly string _path;
        private readonly VsProject _project;

        public bool ContainsErrors { get; private set; }

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

                if (errors.Count > 0)
                {
                    if (DebugLogging.Enable)
                    {
                        OutputWindowMessage.WriteMessage("Script Parser: Script file: \"{0}\" contains errors:", _path);
                    }

                    foreach (var error in errors)
                    {
                        OutputWindowMessage.WriteWarning(_path, error.Line, "Script Parser: Error in {0} error: {1}", _path, error.Message);
                    }

                    ContainsErrors = true;
                }

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
                        OutputWindowMessage.WriteMessage("Unable to read table from the script file: \"{1}\" - error message: \"{1}\" ", _path);                                                
                    }
                }
            }

            if (ContainsErrors)
            {
                OutputWindowMessage.WriteMessage("Errors were encountered parsing the script file - correct errors to save changes");
            }

            return tables;
        }
    }
}
