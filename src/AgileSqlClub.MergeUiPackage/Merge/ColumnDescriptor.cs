using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUI.Merge
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