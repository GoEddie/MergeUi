using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Merge;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace AgileSqlClub.MergeUi.Metadata
{
    public class VsProject
    {
        private readonly string _preDeployScript;
        private readonly string _postDeployScript;
        private readonly HybridDictionary<string, ISchema> _schemas = new HybridDictionary<string, ISchema>();
        private readonly OnDiskMergeRepository _mergeRepository;
        private readonly string _name;
        private readonly DateTime _lastBuildTime;

        public VsProject(string preDeployScript, string postDeployScript, List<ITable> tables, string name, DateTime lastBuildTime)
        {
            _preDeployScript = preDeployScript;
            _postDeployScript = postDeployScript;
            _mergeRepository = new OnDiskMergeRepository(new DacParserBuilder(), this);

            AddSchemas(tables);   
            
            _mergeRepository.OverwriteTablesWithOnDiskData();
            _name = name;
            _lastBuildTime = lastBuildTime;
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
            if (type == ScriptType.PreDeploy)
                return _preDeployScript;

            return _postDeployScript;;
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

        public ITable GetTable(string schemaName, string name)
        {
            var schema = GetSchema(schemaName);
            if (null == schema)
                return null;

            return schema.GetTable(name);

        }

        public void AddTable(string schemaName, ITable table)
        {
            var schema = GetSchema(schemaName);
            if (null == schema)
                return;

            

            schema.AddTable(table);
        }

        public void Save()
        {
            foreach (var schema in _schemas.Values)
            {
                schema.Save(_postDeployScript);
            }
        }

        public DateTime GetLastBuildTime()
        {
            return _lastBuildTime;
        }
    }
}