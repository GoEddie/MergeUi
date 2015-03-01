using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using AgileSqlClub.MergeUi.Extensions;
using AgileSqlClub.MergeUi.Metadata;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Merge
{
    public class MergeStatementParser
    {
        private readonly MergeStatement _merge;

        public MergeStatementParser(MergeStatement merge)
        {
            _merge = merge;
        }

        public ITable GetDescriptor(string filePath, VsProject project)
        {
            var fileContent = File.ReadAllText(filePath);

            var name = (_merge.MergeSpecification.Target as NamedTableReference).SchemaObject.BaseIdentifier.Value;
            var schemaName = (_merge.MergeSpecification.Target as NamedTableReference).SchemaObject.SchemaIdentifier.Value;
            var table = project.GetTable(schemaName, name);
            table.Merge.MergeStatement = _merge;
            table.Merge.ScriptLength = _merge.FragmentLength;
            table.Merge.ScriptOffset = _merge.StartOffset;
            table.Merge.OriginalScript = fileContent.Substring(_merge.StartOffset, _merge.FragmentLength);
            table.Merge.File = filePath;
            

            table.Data = new DataTable(table.Name);
            FillDataTableFromMerge(table);
            return table;
        }

        private void FillDataTableFromMerge(ITable table)
        {
            bool needColumnDescriptors = true;

            var inlineTable = _merge.MergeSpecification.TableReference as InlineDerivedTable;

            if (null == inlineTable)
                return;

            if(table.Columns == null)
                table.Columns =new List<ColumnDescriptor>();

            if (table.Columns.Count > 0)
                needColumnDescriptors = false;

            if (table.Data.Columns.Count <= 0)
            {
                foreach (var col in table.Columns)
                {
                    table.Data.Columns.Add(new DataColumn(col.Name.Value));
                }
            }

            foreach (var row in inlineTable.RowValues)
            {
                if (needColumnDescriptors)
                {
                    MessageBox.Show("erm this is really bad");
                }

                var dataTableRow = table.Data.NewRow();
                int index = 0;

                foreach (var columnValue in row.ColumnValues)
                {
                    if (columnValue as NullLiteral != null)
                    {
                        dataTableRow[index++] = DBNull.Value;
                    }
                    else
                    {
                        dataTableRow[index++] = (columnValue as Literal).Value;    
                    }
                    
                }
                
                table.Data.Rows.Add(dataTableRow);

                needColumnDescriptors = false;  //only need first row
            }

            table.Data.ExtendedProperties.Add(DataTablePropertyNames.DataChanged, false);
            table.Data.EnableDirtyWatcher();
    
        }

    }



    public static class DataTablePropertyNames
    {
        public const string DataChanged = "DataChanged";
    }
}