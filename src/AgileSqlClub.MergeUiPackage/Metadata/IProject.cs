using System;
using System.Collections.Generic;

namespace AgileSqlClub.MergeUi.Metadata
{
    public class VsProject
    {
        private readonly string _preDeployScript;
        private readonly string _postDeployScript;
        private readonly List<ISchema> _schemas;
        private readonly string _name;

        public VsProject(string preDeployScript, string postDeployScript, List<ISchema> schemas, string name)
        {
            _preDeployScript = preDeployScript;
            _postDeployScript = postDeployScript;
            _schemas = schemas;
            _name = name;
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