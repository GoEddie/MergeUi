using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace AgileSqlClub.MergeUi.Metadata
{
    public class VsProject
    {
        private readonly string _preDeployScript;
        private readonly string _postDeployScript;
        private readonly HybridDictionary<string, ISchema> _schemas = new HybridDictionary<string, ISchema>();
        
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
                if(name != null)
                    _schemas[name] = new VsSchema(name, tables.Where(p=>p.SchemaName==name).ToList());
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
            if (_schemas.ContainsKey(name))
                return _schemas[name];

            return null;
        }

        public List<string> GetSchemas()
        {
            return _schemas.Keys.ToList();
        }
    }
}