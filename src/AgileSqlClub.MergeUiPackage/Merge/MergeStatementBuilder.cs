using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUI.Extensions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUI.Merge
{
    public class MergeStatementBuilder
    {
        private readonly List<ColumnDescriptor> _columnDescriptors;
        private readonly List<string> _keyColumns;
        private readonly string _schemaName;
        private readonly string _targetTableName;

        public MergeStatementBuilder(List<ColumnDescriptor> columnDescriptors, string schemaName, string targetTableName, List<string> keyColumns)
        {
            _columnDescriptors = columnDescriptors;
            _schemaName = schemaName;
            _targetTableName = targetTableName;
            _keyColumns = keyColumns;
        }

        public MergeStatement Build()
        {
            var merge = new MergeStatement();
            var specification = merge.MergeSpecification = new MergeSpecification();
            SetTableAlias(specification);

            SetInlineTableReference(specification);

            SetTargetName(specification);

            BuildSearchCondition(specification);

            BuildActions(specification);

            return merge;
        }

        private void BuildActions(MergeSpecification specification)
        {
            BuildInsertAction(specification);
            BuildUpdateAction(specification);
            BuildDeleteAction(specification);
        }

        private void BuildDeleteAction(MergeSpecification specification)
        {
            var action = new DeleteMergeAction();
            specification.ActionClauses.Add(new MergeActionClause
            {
                Action = action,
                Condition = MergeCondition.NotMatchedBySource
            });
        }

        private void BuildUpdateAction(MergeSpecification specification)
        {
            var clause = new MergeActionClause();
            var expression =
                (clause.SearchCondition = new BooleanParenthesisExpression()) as BooleanParenthesisExpression;

            var isNulls = BuildNullIfStatements();

            if (isNulls.Count > 1)
            {
                var expressions = CreateExpressionTreeForUpdateSearch(isNulls);

                //Save the first expression
                expression.Expression = expressions.First();
                clause.SearchCondition = expression;
            }
            else
            {
                clause.SearchCondition = isNulls[0];
            }
            clause.Condition = MergeCondition.Matched;
            clause.Action = CreateUpdateSetActions(clause);

            specification.ActionClauses.Add(clause);
        }

        private UpdateMergeAction CreateUpdateSetActions(MergeActionClause clause)
        {
            var action = (clause.Action = new UpdateMergeAction()) as UpdateMergeAction;
            foreach (var col in _columnDescriptors)
            {
                var setClause = new AssignmentSetClause();
                setClause.AssignmentKind = AssignmentKind.Equals;

                var identifier = new MultiPartIdentifier().Create(MergeIdentifierStrings.TargetName, col.Name);

                setClause.Column = new ColumnReferenceExpression();
                setClause.Column.MultiPartIdentifier = identifier;

                var newValue = (setClause.NewValue = new ColumnReferenceExpression()) as ColumnReferenceExpression;
                newValue.MultiPartIdentifier = new MultiPartIdentifier().Create(MergeIdentifierStrings.SourceName, col.Name);


                action.SetClauses.Add(setClause);
            }
            return action;
        }

        private static List<BooleanBinaryExpression> CreateExpressionTreeForUpdateSearch(List<BooleanIsNullExpression> isNulls)
        {
            var expressions = new List<BooleanBinaryExpression>();
            BooleanBinaryExpression last = null;
            var counter = 0;
            
            foreach (var isNull in isNulls)
            {
                var boolExpression = new BooleanBinaryExpression
                {
                    SecondExpression = isNull,
                    BinaryExpressionType = BooleanBinaryExpressionType.Or
                };

                if (last != null)
                {
                    if (isNulls.Count - 1 == ++counter)
                    {
                        last.FirstExpression = isNulls.Last();
                    }
                    else
                    {
                        last.FirstExpression = boolExpression;
                    }
                }

                last = boolExpression;
                expressions.Add(last);
            }

            return expressions;
        }

        //The isNulls are used in the search condition to find out if any of the columns are different and therefore need an update
        private List<BooleanIsNullExpression> BuildNullIfStatements()
        {
            var isNulls = new List<BooleanIsNullExpression>();

            foreach (var descriptor in _columnDescriptors)
            {
                var nullExpression = new NullIfExpression();

                var first =
                    (nullExpression.FirstExpression = new ColumnReferenceExpression()) as ColumnReferenceExpression;
                first.MultiPartIdentifier = new MultiPartIdentifier().Create(MergeIdentifierStrings.SourceName, descriptor.Name);

                var second =
                    (nullExpression.SecondExpression = new ColumnReferenceExpression()) as ColumnReferenceExpression;
                second.MultiPartIdentifier = new MultiPartIdentifier().Create(MergeIdentifierStrings.TargetName, descriptor.Name);

                var isNullExpresson = new BooleanIsNullExpression();
                isNullExpresson.Expression = nullExpression;
                isNullExpresson.IsNot = true;
                isNulls.Add(isNullExpresson);
            }

            return isNulls;
        }

        private void BuildInsertAction(MergeSpecification specification)
        {
            var action = new InsertMergeAction();
            var insertSource = action.Source = new ValuesInsertSource();
            var row = new RowValue();

            foreach (var column in _columnDescriptors)
            {
                var colRef = new ColumnReferenceExpression();
                colRef.ColumnType = ColumnType.Regular;
                colRef.MultiPartIdentifier = new MultiPartIdentifier().Create(column.Name);
                action.Columns.Add(colRef);

                colRef = new ColumnReferenceExpression();
                colRef.ColumnType = ColumnType.Regular;
                colRef.MultiPartIdentifier = new MultiPartIdentifier().Create(new Identifier { Value = MergeIdentifierStrings.SourceName },
                    column.Name);

                row.ColumnValues.Add(colRef);
            }

            insertSource.RowValues.Add(row);

            specification.ActionClauses.Add(new MergeActionClause
            {
                Action = action,
                Condition = MergeCondition.NotMatchedByTarget
            });
        }

        private void SetTableAlias(MergeSpecification specification)
        {
            specification.TableAlias = new Identifier { Value = MergeIdentifierStrings.TargetName };
        }

        private void SetInlineTableReference(MergeSpecification specification)
        {
            var table = (specification.TableReference = new InlineDerivedTable()) as InlineDerivedTable;
            table.Alias = new Identifier { Value = MergeIdentifierStrings.SourceName };
            foreach (var col in _columnDescriptors)
            {
                table.Columns.Add(col.Name);
            }
        }

        private void SetTargetName(MergeSpecification specification)
        {
            var table = (specification.Target = new NamedTableReference()) as NamedTableReference;
            table.SchemaObject = (new SchemaObjectName().Create(_schemaName, _targetTableName) as SchemaObjectName);
        }

        private void BuildSearchCondition(MergeSpecification specification)
        {
            if (_keyColumns.Count > 1)
            {
                BuildMultiKeySearchCondition(specification);
                return;
            }

            CreateSearchCondition(_keyColumns[0], (specification.SearchCondition = new BooleanComparisonExpression()) as BooleanComparisonExpression);
        }

        private BooleanComparisonExpression CreateSearchCondition(string keyColumn, BooleanComparisonExpression condition)
        {

            condition.ComparisonType = BooleanComparisonType.Equals;
            condition.FirstExpression = new ScalarExpressionSnippet { Script = string.Format("{0}.{1}", MergeIdentifierStrings.SourceName, keyColumn) };
            condition.SecondExpression = new ScalarExpressionSnippet { Script = string.Format("{0}.{1}", MergeIdentifierStrings.TargetName, keyColumn) };
            return condition;
        }

        private void BuildMultiKeySearchCondition(MergeSpecification specification)
        {
            var comparisons = new List<BooleanComparisonExpression>();
            foreach (var column in _keyColumns)
            {
                var condition = new BooleanComparisonExpression();
                comparisons.Add(CreateSearchCondition(column, condition));
            }

            specification.SearchCondition = CreateExpressionTreeForMultiKeySearchConditon(comparisons).First();
        }

        private static List<BooleanBinaryExpression> CreateExpressionTreeForMultiKeySearchConditon(List<BooleanComparisonExpression> booleanComparisons)
        {
            var expressions = new List<BooleanBinaryExpression>();
            BooleanBinaryExpression last = null;
            var counter = 0;
            foreach (var isNull in booleanComparisons)
            {
                var boolExpression = new BooleanBinaryExpression
                {
                    SecondExpression = isNull,
                    BinaryExpressionType = BooleanBinaryExpressionType.And
                };

                if (last != null)
                {
                    if (booleanComparisons.Count - 1 == ++counter)
                    {
                        last.FirstExpression = booleanComparisons.Last();
                    }
                    else
                    {
                        last.FirstExpression = boolExpression;
                    }
                }

                last = boolExpression;
                expressions.Add(last);
            }

            return expressions;
        }
    }


    public class MergeIdentifierStrings
    {
        public const string TargetName = "TARGET";
        public const string SourceName = "SOURCE";
    }
}
