using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.PackagePlumbing;
using AgileSqlClub.MergeUi.VSServices;
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.Shell.Interop;

namespace AgileSqlClub.MergeUi.UI
{
    /// <summary>
    /// Interaction logic for MergeUi.xaml
    /// </summary>
    public partial class MergeUi : UserControl, IStatus
    {
        private SolutionParser _solution;
        private VsProject _currentProject;
        private ISchema _currentSchema;
        private ITable _currentTable;
        
        public MergeUi()
        {
            InitializeComponent();
        }

        public void SetStatus(string message)
        {
            Dispatcher.InvokeAsync(() => { StatusLabel.Text = string.Format("Status: {0}",message); });
        }

        private void ClearAll()
        {
            Dispatcher.Invoke(() =>
            {
                Projects.ItemsSource = null;
                Schemas.ItemsSource = null;
                Tables.ItemsSource = null;
                Table.DataContext = null;
                Import.IsEnabled = false;
            });
        }

        private void ClearProject()
        {
            Dispatcher.Invoke(() =>
            {
                Schemas.ItemsSource = null;
                Tables.ItemsSource = null;
                Table.DataContext = null;
                Import.IsEnabled = false;
            });
        }

        private void ClearSchema()
        {
            Dispatcher.Invoke(() =>
            {
                Tables.ItemsSource = null;
                Table.DataContext = null;
                Import.IsEnabled = false;
            });
        }

        private void ClearTable()
        {
            Dispatcher.Invoke(() =>
            {

                Import.IsEnabled = false;
                Table.DataContext = null;
            });
        }



        private void Populate_OnClick(object sender, RoutedEventArgs e)
        {
            var cursor = Cursors.Arrow;

            Dispatcher.Invoke(() =>
            {
                cursor = Cursor;            
                Cursor = Cursors.Wait;
            });

            ClearAll();

            _solution = new SolutionParser(new ProjectEnumerator(), new DacParserBuilder(), this);

            Dispatcher.Invoke(() =>
            {
                Projects.ItemsSource = _solution.GetProjects();
                Cursor = cursor;
                Populate.IsEnabled = true;
            });
        }

        private void Projects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearProject();

            if (Projects.SelectedValue == null)
                return;

            var projectName = Projects.SelectedValue.ToString();

            if (String.IsNullOrEmpty(projectName))
                return;

            Schemas.ItemsSource = null;
            Tables.ItemsSource = null;

            _currentProject = _solution.GetProject(projectName);

            if (string.IsNullOrEmpty(_currentProject.GetScript(ScriptType.PostDeploy)))
            {
                MessageBox.Show(
                    "The project needs a post deploy script - add one anywhere in the project and refresh", "MergeUi");
                return;
            }

            
            Schemas.ItemsSource = _currentProject.GetSchemas(); 
        }

        private void Schemas_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearSchema();

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
            ClearTable();

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

            Table.DataContext = _currentTable.Data.DefaultView;
            //TODO -= check for null and start adding a datatable when building the table (maybe need a lazy loading)
            //we also need a repository of merge statements which is the on disk representation so we can grab those
            //if they exist or just create a new one - then save them back and 


            Import.IsEnabled = true;
        }

        private void Import_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentTable == null)
            {
                MessageBox.Show("Please choose a table in the drop down list", "MergeUi");
                return;
            }
            try
            {
                new Importer().GetData(_currentTable);
            }
            catch (Exception ex)
            {
                SetStatus("Error see output window");

                OutputWindowMessage.WriteMessage("Error importing data (table={0}):", _currentTable.Name);
                OutputWindowMessage.WriteMessage(ex.Message);
            }

            Table.DataContext = _currentTable.Data.DefaultView;
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _solution.Save();
            }
            catch (Exception ex)
            {
                OutputWindowMessage.WriteMessage("Error saving the changes: {0}", ex.Message);
            }
        }

    }
}
