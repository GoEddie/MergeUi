using System.Collections.Generic;

namespace AgileSqlClub.MergeUI.Metadata
{
    public interface ISchema
    {
        ITable GetTable(string name);
        List<string> GetTables();

        void AddTable(ITable table);

        string GetName();

        void Save(string scriptFile);

        
    }
}