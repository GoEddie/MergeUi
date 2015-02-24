using System;
using System.IO;
using System.Web.UI.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.VSServices;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace AgileSqlClub.MergeUi.UI
{

    public partial class MyControl : UserControl
    {
        private ISolution _solution;
        private VsProject _currentProject;
        private ISchema _currentSchema;
        private ITable _currentTable;

        public MyControl()
        {
            InitializeComponent();
            
            Refresh();
        }

        void Refresh()
        {
            if (_currentDataGridDirty)
            {
                if (!CheckSaveChanges())
                {
                    return;
                }
            }


            Projects.ItemsSource = null;
            Schemas.ItemsSource = null;
            Tables.ItemsSource = null;
            DataGrid.DataContext = null;

            this.Dispatcher.InvokeAsync(() =>
            {
                var cursor = Cursor;
                Cursor = System.Windows.Input.Cursors.Wait;
                _solution = new Solution(new ProjectEnumerator(), new DacParserBuilder());
                Projects.ItemsSource = _solution.GetProjects();
                
                Cursor = cursor;
            });

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

            
        private void Projects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == Projects.SelectedValue)
                return;

            var projectName = Projects.SelectedValue.ToString();

            if (String.IsNullOrEmpty(projectName))
                return;

            Schemas.ItemsSource = null;
            Tables.ItemsSource = null;

            _currentProject = _solution.GetProject(projectName);
            Schemas.ItemsSource = _currentProject.GetSchemas();
        }

        private void Schemas_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == Schemas.SelectedValue)
                return;

            var schemaName = Schemas.SelectedValue.ToString();

            if (String.IsNullOrEmpty(schemaName))
                return;

            _currentSchema = _currentProject.GetSchema(schemaName);

            Tables.ItemsSource = _currentSchema.GetTables();

            
        }

        private void Tables_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (null == Tables.SelectedValue)
                return;

            var tableName = Tables.SelectedValue.ToString();

            if (String.IsNullOrEmpty(tableName))
                return;

            _currentTable = _currentSchema.GetTable(tableName);

            if (_currentTable.Data == null)
            {
                _currentTable.Data = new DataTableBuilder(tableName, _currentTable.Columns).Get();
                
            }

            DataGrid.DataContext = _currentTable.Data.DefaultView;  //TODO -= check for null and start adding a datatable when building the table (maybe need a lazy loading)
                                                                    //we also need a repository of merge statements which is the on disk representation so we can grab those
                                                                    //if they exist or just create a new one - then save them back and 

        }

        private bool CheckSaveChanges()
        {
            MessageBox.Show("Existing: " +
                            new StreamReader(_currentTable.Merge.File).ReadToEnd()
                                .Substring(_currentTable.Merge.ScriptOffset, _currentTable.Merge.ScriptLength));

            return true;
        }

        private bool _currentDataGridDirty;

        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            _currentDataGridDirty = true;
        }

        private void button1_Save(object sender, RoutedEventArgs e)
        {
            //need to finish off saving back to the files (need a radio button with pre/post deploy (not changeable when read from file) - futrue feature
            //need a check to write files on window closing
            //need lots of tests
            throw new NotImplementedException();
        }
    }
}