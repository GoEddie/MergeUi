﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Merge
{
    public static class MergeStatementDesriptorToScriptExtensions
    {
        public static string GetScript(this MergeStatement source)
        {
            var script = "";
            var generator =
                new Sql120ScriptGenerator(new SqlScriptGeneratorOptions
                {
                    IncludeSemicolons = true,
                    AlignClauseBodies = true,
                    AlignColumnDefinitionFields = true,
                    AsKeywordOnOwnLine = true,
                    MultilineInsertSourcesList = true,
                    MultilineInsertTargetsList = true,
                    MultilineSelectElementsList = true,
                    NewLineBeforeOpenParenthesisInMultilineList = true
                });
            generator.GenerateScript(source, out script);

            if (!script.EndsWith(";"))
            {
                script = script + ";";
            }
            return script;
        }

        public static void SetInlineTableData(this MergeStatement source, DataTable data, List<ColumnDescriptor> columns)
        {
            var table = source.MergeSpecification.TableReference as InlineDerivedTable;

            if (null == table)
                throw new NotImplementedException("only support merges from inline table reference");

            table.RowValues.Clear();

            var columnsToRemove = new List<DataColumn>();
            for (var i = 0; i < data.Columns.Count; i++)
            {
                if (columns.FirstOrDefault(p => p.Name.Value == data.Columns[i].ColumnName) == null)
                    columnsToRemove.Add(data.Columns[i]);
            }

            foreach (var column in columnsToRemove)
            {
                data.Columns.Remove(column);
            }

            foreach (DataRow row in data.Rows)
            {
                var rowValue = new RowValue();
                for (var i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row.ItemArray[i] == DBNull.Value)
                    {
                        rowValue.ColumnValues.Add(new NullLiteral());
                    }
                    else
                    {
                        rowValue.ColumnValues.Add(GetColumnValue(row.ItemArray[i].ToString(), columns[i].LiteralType));
                    }
                }

                table.RowValues.Add(rowValue);
            }
        }

        private static ScalarExpression GetColumnValue(string value, LiteralType type)
        {
            switch (type)
            {
                case LiteralType.Integer:
                    return new IntegerLiteral {Value = value};
                case LiteralType.Real:
                    return new RealLiteral {Value = value};
                case LiteralType.Money:
                    return new MoneyLiteral {Value = value};
                case LiteralType.Binary:
                    return new BinaryLiteral {Value = value};
                case LiteralType.String:
                    return new StringLiteral {Value = value.Replace("'", "''")};
                case LiteralType.Null:
                    return new NullLiteral {Value = value};
                case LiteralType.Default:
                    return new DefaultLiteral {Value = value};
                case LiteralType.Max:
                    return new MaxLiteral {Value = value};
                case LiteralType.Odbc:
                    return new OdbcLiteral {Value = value};
                case LiteralType.Identifier:
                    return new IdentifierLiteral {Value = value};
                    ;
                case LiteralType.Numeric:
                    return new NumericLiteral {Value = value};
                    ;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}