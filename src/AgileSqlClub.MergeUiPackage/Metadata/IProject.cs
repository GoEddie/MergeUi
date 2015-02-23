using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileSqlClub.MergeUi.Metadata
{
    public class VsProject
    {
        private readonly string _preDeployScript;
        private readonly string _postDeployScript;
        private readonly List<ISchema> _schemas = new List<ISchema>();
        private readonly string _name;

        public VsProject(string preDeployScript, string postDeployScript, List<ITable> tables, string name)
        {
            _preDeployScript = preDeployScript;
            _postDeployScript = postDeployScript;
            AddSchemas(tables);   
            _name = name;
        }

        private void AddSchemas(List<ITable> tables)
        {
            foreach (string name in tables.Select(p => p.SchemaName).Distinct())
            {
                _schemas.Add(new VsSchema(name, tables));
            }
        }

        public string GetName()
        {
            return _name;
        }

        public string GetScript(ScriptType type)
        {
            throw new NotImplementedException();
        }

        public void SetScript(ScriptType type, string script)
        {
            throw new NotImplementedException();
        }

        public ISchema GetSchema(string name)
        {
            throw new NotImplementedException();
        }

        public List<string> GetSchemas()
        {
            throw new NotImplementedException();
        }
    }
}