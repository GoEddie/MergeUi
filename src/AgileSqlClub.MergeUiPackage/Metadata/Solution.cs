using System.Collections.Generic;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.VSServices;

namespace AgileSqlClub.MergeUi.Metadata
{
    public class Solution : ISolution
    {
        private readonly List<string> _projectList = new List<string>();
        private readonly Dictionary<string, VsProject> _projects = new Dictionary<string, VsProject>();

        public Solution(ProjectEnumerator projectEnumerator, DacParserBuilder dacParserBuilder)
        {
            foreach (var project in projectEnumerator.EnumerateProjects())
            {
                var dac = dacParserBuilder.Build(project.DacPath);

                _projectList.Add(project.Name);
                _projects.Add(project.Name, new VsProject(project.PreDeployScriptPath, project.PostDeployScriptPath, dac.GetTableDefinitions(), project.Name));
            }
        }
        
        public VsProject GetProject(string name)
        {
            if (_projects.ContainsKey(name))
                return _projects[name];

            return null;
        }

        public List<string> GetProjects()
        {
            return _projectList;
        }

        public ITable GetTable(string projectName, string schemaName, string tableName)
        {
            var project = GetProject(projectName);

            if (null == project)
                return null;

            var schema = project.GetSchema(schemaName);

            if (null == schema)
                return null;

            var table = schema.GetTable(tableName);
            
            return table;
        }

        public void AddTable(string projectName, string schemaName, string tableName, ITable table)
        {
            var project = GetProject(projectName);
            if (null == project)
                return;

            var schema = project.GetSchema(schemaName);
            if (null == schema)
                return;

            schema.AddTable(table);
            
        }

        public void Save()
        {
            foreach (var project in _projects.Values)
            {
                project.Save();
            }
        }
    }
}