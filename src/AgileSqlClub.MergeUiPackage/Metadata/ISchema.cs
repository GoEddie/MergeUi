using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface ISchema
    {
        ITable GetTable(string name);
        List<string> GetTables();

        void AddTable(ITable table);

        string GetName();
    }
}