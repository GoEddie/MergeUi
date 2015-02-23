using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.DacServices;
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
                _projects.Add(project.Name, new VsProject(dac.PreDeployScript, dac.PostDeployScript, dac.GetTableDefinitions(), project.Name));
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
    }

    internal class VsSchema : ISchema
    {
        private readonly string _schemaName;
        private readonly IList<string> _tableNames;
        private readonly IList<ITable> _tables; 

        public VsSchema(string schemaName, IList<ITable> tables)
        {
            _schemaName = schemaName;
            _tables = tables;
            _tableNames = tables.Select(p=>p.SchemaName).ToList();
        }

        public ITable GetTable(string name)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTables()
        {
            throw new NotImplementedException();
        }

        public void AddTable(ITable table)
        {
            throw new NotImplementedException();
        }
    }
}
