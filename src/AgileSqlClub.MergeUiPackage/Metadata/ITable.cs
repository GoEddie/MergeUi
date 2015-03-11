using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AgileSqlClub.MergeUI.Extensions;
using AgileSqlClub.MergeUI.Merge;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using AgileSqlClub.MergeUI.UI;
using AgileSqlClub.MergeUI.PackagePlumbing;

namespace AgileSqlClub.MergeUI.Metadata
{
    public interface ITable
    {
        string SchemaName { get; set; }
        string Name { get; set; }
        List<ColumnDescriptor> Columns { get; set; }
        List<string> KeyColumns { get; set; }

        DataTable Data { get; set; }
        Merge Merge { get; set; }

        void Save(string scriptFile);
    }

    public class Merge
    {
        public MergeStatement MergeStatement { get; set; }
        public string File { get; set; }
        public string OriginalScript { get; set; }
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

        public void Save(string scriptFile)
        {
            if (Data != null && Data.IsDirty())
            {

                if (DebugLogging.Enable)
                {
                    OutputWindowMessage.WriteMessage("Table: Saving change to table: \"{0}\" to scriptfile: \"{1}\"", Name, scriptFile );
                }

                //if detils of Merge.Blah are filled in then update the current Merge.MergeStatement with the new datatable and then get the script and overwrite the existing script..
                //if it is not filled in, we need to create a new one and build a new merge 
                Data.AcceptChanges();

                if (Merge.MergeStatement == null)
                {
                    Merge.MergeStatement = new MergeStatementBuilder(Columns, SchemaName, Name, KeyColumns).Build();
                    Merge.File = scriptFile;
                }

                Merge.MergeStatement.SetInlineTableData(Data, Columns);
                var script = Merge.MergeStatement.GetScript();
                
                string originalScript = null;
                using (var sr = new StreamReader(Merge.File))
                {
                    originalScript = sr.ReadToEnd();
                }

                string outputScript = null;

                if (!string.IsNullOrEmpty(Merge.OriginalScript))
                {
                    outputScript = originalScript.Replace(Merge.OriginalScript, "");
                }
                else
                {
                    outputScript = originalScript;
                }

                Merge.OriginalScript = script;

                outputScript = outputScript + "\r\nGO\r\n" + script;
                outputScript = outputScript.Replace("\r\nGO\r\n\r\nGO\r\n", "\r\nGO\r\n");

                using (var sw = new StreamWriter(Merge.File,false))
                {
                    sw.Write(outputScript);
                }
                
                Data.SetClean();
            }
        }
    }

}