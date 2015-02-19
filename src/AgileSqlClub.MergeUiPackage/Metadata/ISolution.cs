using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Metadata
{
    public interface ISolution
    {
        IProject GetProject(string name);
        List<string> GetProjects();
    }
}