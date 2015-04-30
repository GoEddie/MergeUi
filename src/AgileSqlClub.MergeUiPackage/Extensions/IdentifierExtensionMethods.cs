using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AgileSqlClub.MergeUi.Extensions
{
    public static class IdentifierExtensionMethods
    {
        public static MultiPartIdentifier Create(this MultiPartIdentifier src, params string[] names)
        {
            foreach (var name in names)
            {
                src.Identifiers.Add(new Identifier() { Value = name });
            }

            return src;
        }

        public static MultiPartIdentifier Create(this MultiPartIdentifier src, params Identifier[] names)
        {
            foreach (var name in names)
            {
                src.Identifiers.Add(name);
            }

            return src;
        }

        public static MultiPartIdentifier Create(this MultiPartIdentifier src, string id1, Identifier id2)
        {

            src.Identifiers.Add(new Identifier() { Value = id1 });
            src.Identifiers.Add(id2);
            return src;
        }
    }
}