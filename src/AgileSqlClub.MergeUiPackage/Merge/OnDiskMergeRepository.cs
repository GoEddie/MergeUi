using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;

namespace AgileSqlClub.MergeUi.Merge
{
    public enum DataTableSource
    {
        PreDeployScript,
        PostDeployScript,
        New
    }

    public class OnDiskMergeRepository
    {
        
        private readonly DacParserBuilder _dacParserBuilder;
        private readonly VsProject _project;
        
        private Dictionary<string, ScriptFileGateway> _scriptGateways = new Dictionary<string, ScriptFileGateway>(); 

        public OnDiskMergeRepository(DacParserBuilder dacParserBuilder, VsProject project)
        {
            _dacParserBuilder = dacParserBuilder;
            _project = project;

            BuildScriptGateways();
        }

        private void BuildScriptGateways()
        {
                var path = _project.GetScript(ScriptType.PreDeploy);
                
                _scriptGateways["Pre"] = new ScriptFileGateway(_project, path);
            
                path = _project.GetScript(ScriptType.PostDeploy);
                _scriptGateways["Post"] = new ScriptFileGateway(_project, path);
            
        }

        public void OverwriteTablesWithOnDiskData()
        {
            //need to set extended property to source (source = file, start pos end pos etc

            foreach (var scriptGateway in _scriptGateways.Values)
            {
                scriptGateway.UpdateData();
            }
            
        }

    }

    public class ScriptFileGateway
    {
        private readonly VsProject _project;
        private readonly string _path;
        private readonly List<ITable> _tables = new List<ITable>();
        private DateTime _lastScriptSaveTime = DateTime.MinValue;

        public ScriptFileGateway(VsProject project, string path)
        {
            _project = project;
            _path = path;
        }

        public void UpdateData()
        {
            if (!FileHasChanged())
                return;

            //do the gubbins here...
            GetTables();
            _lastScriptSaveTime = File.GetLastWriteTime(_path);
        }
        
        private void GetTables()
        {
            var tables = new ScriptParser(_path).GetDataTables();
            foreach (var table in tables)
            {
                _project.AddTable(table.SchemaName, table);
            }
        }

        private bool FileHasChanged()
        {
            var lastWriteTime = File.GetLastWriteTime(_path);

            return lastWriteTime > _lastScriptSaveTime;
        }
    }
}
