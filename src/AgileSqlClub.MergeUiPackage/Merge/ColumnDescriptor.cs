using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Merge
{
    public class ColumnDescriptor
    {
        public ColumnDescriptor()
        {
            
        }
        public LiteralType LiteralType;
        public Identifier Name; 
    }
}