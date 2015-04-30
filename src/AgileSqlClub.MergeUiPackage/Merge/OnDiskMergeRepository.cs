using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.PackagePlumbing;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;
using System.Windows.Forms;

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

        public bool CanSave()
        {
            return _scriptGateways.All(sg => sg.Value.CanSave());
        }

        private void BuildScriptGateways()
        {
                var path = _project.GetScript(ScriptType.PreDeploy);

                if(!string.IsNullOrEmpty(path))       
                    _scriptGateways["Pre"] = new ScriptFileGateway(_project, path);
            

                path = _project.GetScript(ScriptType.PostDeploy);

                if (!string.IsNullOrEmpty(path))       
                    _scriptGateways["Post"] = new ScriptFileGateway(_project, path);
            
        }

        public void OverwriteTablesWithOnDiskData()
        {
            //need to set extended property to source (source = file, start pos end pos etc

            foreach (var scriptGateway in _scriptGateways.Values)
            {
                scriptGateway.UpdateData();
            }

            //if (!CanSave())
            //{
            //    MessageBox.Show(
            //        "Errors in the post-dpeloy script, fix them before editing anything. You won't be able to save but can view the data we have read.\r\n\n\nFor further info, enable debugging logging and refresh", "MergeUi");
            //}
        }

    }

    public class ScriptFileGateway
    {
        private readonly VsProject _project;
        private readonly string _path;
        private readonly List<ITable> _tables = new List<ITable>();
        private DateTime _lastScriptSaveTime = DateTime.MinValue;

        private bool _doNotSave = false;


        public ScriptFileGateway(VsProject project, string path)
        {
            _project = project;
            _path = path;
            
        }

        public bool CanSave()
        {
            return !_doNotSave;
        }

        public void UpdateData()
        {
            if (_doNotSave)
            {
                MessageBox.Show(string.Format("Unable to save changes to: \"{0}\" as it contained errors, correct these, click refresh then try again", _path), "MergeUi");
                return;
            }

            if (!FileHasChanged())
                return;

            //do the gubbins here...
            GetTables();
            _lastScriptSaveTime = File.GetLastWriteTime(_path);
        }
        
        private void GetTables()
        {
            var parser = new ScriptParser(_path, _project);
            var tables = parser.GetDataTables();
            
            foreach (var table in tables)
            {                
                _project.AddTable(table.SchemaName, table);
            }

            _doNotSave = parser.ContainsErrors;
        }

        private bool FileHasChanged()
        {
            var lastWriteTime = File.GetLastWriteTime(_path);

            var ret = lastWriteTime > _lastScriptSaveTime;

            _lastScriptSaveTime = File.GetLastWriteTime(_path);

            return ret;
        }
    }
}
