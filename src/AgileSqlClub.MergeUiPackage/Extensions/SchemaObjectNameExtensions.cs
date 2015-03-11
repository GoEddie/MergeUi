using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUI.Extensions
{
    public static class SchemaObjectNameExtensions
    {
        public static string GetName(this SchemaObjectName name)
        {
            var builder = new StringBuilder();

            AddPart(builder, name.DatabaseIdentifier, false);
            AddPart(builder, name.SchemaIdentifier, false);
            AddPart(builder, name.BaseIdentifier, true);

            return builder.ToString();
        }

        public static string GetSchema(this SchemaObjectName name)
        {
            var builder = new StringBuilder();

            AddPart(builder, name.DatabaseIdentifier, false);
            AddPart(builder, name.SchemaIdentifier, false);
            AddPart(builder, name.BaseIdentifier, true);

            return builder.ToString();
        }



        private static void AddPart(StringBuilder builder, Identifier part, bool isLast)
        {
            if (part == null)
                return;

            if (isLast)
                builder.Append(part.Value);
            else
                builder.AppendFormat("{0}.", part.Value);
        }
    }
}