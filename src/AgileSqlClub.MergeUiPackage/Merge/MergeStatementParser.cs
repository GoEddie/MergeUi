using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AgileSqlClub.MergeUI.Extensions;
using AgileSqlClub.MergeUI.Metadata;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUI.Merge
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
            var schemaName =
                (_merge.MergeSpecification.Target as NamedTableReference).SchemaObject.SchemaIdentifier.Value;
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
            var inlineTable = _merge.MergeSpecification.TableReference as InlineDerivedTable;

            if (null == inlineTable)
                return;

            if (table.Columns == null)
                table.Columns = new List<ColumnDescriptor>();

            if (table.Data.Columns.Count <= 0)
            {
                foreach (var col in table.Columns)
                {
                    table.Data.Columns.Add(new DataColumn(col.Name.Value));
                }
            }

            var rowIndex = 1;

            foreach (var row in inlineTable.RowValues)
            {
                var dataTableRow = table.Data.NewRow();
                var index = 0;

                foreach (var columnValue in row.ColumnValues)
                {
                    if (columnValue as NullLiteral != null)
                    {
                        dataTableRow[index++] = DBNull.Value;
                    }
                    else
                    {
                        if (columnValue as Literal == null)
                        {
                            throw new MergeStatamentParsingException(table.Name, index+1, columnValue, rowIndex);
                        }

                        dataTableRow[index++] = (columnValue as Literal).Value;
                    }
                }

                table.Data.Rows.Add(dataTableRow);

                rowIndex++;
            }

            table.Data.ExtendedProperties.Add(DataTablePropertyNames.DataChanged, false);
            table.Data.EnableDirtyWatcher();
        }
    }

    public class MergeStatamentParsingException : Exception
    {
        public MergeStatamentParsingException(string name, int ordinal, ScalarExpression columnValue, int rowIndex)
            : base(string.Format("Could not parse column ordinal: {1} in table: {0}, row number: {3} extra info:  base type: {2}",
                name, ordinal, columnValue.GetType(), rowIndex))
        {
        }
    }


    public static class DataTablePropertyNames
    {
        public const string DataChanged = "DataChanged";
        public const string DoNotSave = "DoNotSave";
    }
}