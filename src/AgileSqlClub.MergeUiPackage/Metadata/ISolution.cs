using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface ISolution
    {
        VsProject GetProject(string name);
        List<string> GetProjects();

        ITable GetTable(string projectName, string schemaName, string tableName);

        void AddTable(string projectName, string schemaName, string tableName, ITable table);
        void Save();
    }
}