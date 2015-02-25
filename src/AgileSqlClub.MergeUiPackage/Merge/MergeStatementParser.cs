using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public ITable GetDescriptor(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);

            var table = new Table
            {
                Name = (_merge.MergeSpecification.Target as NamedTableReference).SchemaObject.BaseIdentifier.Value,
                SchemaName =(_merge.MergeSpecification.Target as NamedTableReference).SchemaObject.SchemaIdentifier.Value,
                Merge =
                {
                    MergeStatement = _merge,
                    ScriptOffset = _merge.StartOffset,
                    ScriptLength = _merge.FragmentLength,
                    OriginalScript =  fileContent.Substring(_merge.StartOffset, _merge.FragmentLength),
                    File = filePath
                }
            };


            table.Data = new DataTable(table.Name);
            table = DataTableFromMerge(table);
            return table;
        }

        private Table DataTableFromMerge(Table table)
        {
            bool needColumnDescriptors = true;

            var inlineTable = _merge.MergeSpecification.TableReference as InlineDerivedTable;

            if (null == inlineTable)
                return null;

            if(table.Columns == null)
                table.Columns =new List<ColumnDescriptor>();

            foreach (var row in inlineTable.RowValues)
            {
                if (needColumnDescriptors)
                {
                    foreach (var columnValue in row.ColumnValues)
                    {
                        var val = columnValue as Literal;

                        if (val == null)
                            throw new NotImplementedException("couldn't convert value to a literal");

                        table.Columns.Add(new ColumnDescriptor { LiteralType = val.LiteralType });

                        table.Data.Columns.Add(new DataColumn());
                    }
                }

                var dataTableRow = table.Data.NewRow();
                int index = 0;

                foreach (var columnValue in row.ColumnValues)
                {
                    dataTableRow[index++] = (columnValue as Literal).Value;
                }
                
                table.Data.Rows.Add(dataTableRow);

                needColumnDescriptors = false;  //only need first row
            }

            table.Data.ExtendedProperties.Add(DataTablePropertyNames.DataChanged, false);
            table.Data.EnableDirtyWatcher();
    
            return table;
        }

    }



    public static class DataTablePropertyNames
    {
        public const string DataChanged = "DataChanged";
    }
}