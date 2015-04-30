using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Merge;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.PackagePlumbing;
using AgileSqlClub.MergeUi.VSServices;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AgileSqlClub.MergeUi.UI
{
    public static class DebugLogging
    {
        public static bool Enable = true;
    }

    public partial class MyControl : UserControl, IStatus
    {
        private bool _currentDataGridDirty;
        private VsProject _currentProject;
        private ISchema _currentSchema;
        private ITable _currentTable;
        private ISolution _solution;

        public MyControl()
        {
            InitializeComponent();

            //Refresh();
        }

        public void SetStatus(string message)
        {
            Dispatcher.InvokeAsync(() => { LastStatusMessage.Text = message; });
        }

        private void Refresh()
        {
            Task.Run(() => DoRefresh());
        }

        private void DoRefresh()
        {

            Dispatcher.Invoke(() => { DebugLogging.Enable = Logging.IsChecked.Value; });
            try
            {
                if (_currentDataGridDirty)
                {
                    if (!CheckSaveChanges())
                    {
                        return;
                    }
                }

                var cursor = Cursors.Arrow;

                Dispatcher.Invoke(() =>
                {
                    cursor = Cursor;
                    RefreshButton.IsEnabled = false;
                    Projects.ItemsSource = null;
                    Schemas.ItemsSource = null;
                    Tables.ItemsSource = null;
                    DataGrid.DataContext = null;

                    Cursor = Cursors.Wait;
                });

                _solution = new SolutionParser(new ProjectEnumerator(), new DacParserBuilder(), this);

                Dispatcher.Invoke(() =>
                {
                    Projects.ItemsSource = _solution.GetProjects();
                    Cursor = cursor;
                    RefreshButton.IsEnabled = true;
                });
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error see output window"; });

                OutputWindowMessage.WriteMessage("Error Enumerating projects:");
                OutputWindowMessage.WriteMessage(e.Message);
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Projects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (null == Projects.SelectedValue)
                    return;

                var projectName = Projects.SelectedValue.ToString();

                if (String.IsNullOrEmpty(projectName))
                    return;

                Schemas.ItemsSource = null;
                Tables.ItemsSource = null;

                _currentProject = _solution.GetProject(projectName);

                if (string.IsNullOrEmpty(_currentProject.GetScript(ScriptType.PreDeploy)) &&
                    string.IsNullOrEmpty(_currentProject.GetScript(ScriptType.PostDeploy)))
                {
                    MessageBox.Show(
                        "The project needs a post deploy script - add one anywhere in the project and refresh", "MergeUi");
                    return;
                }

                LastBuildTime.Text = string.Format("Last Dacpac Build Time: {0}", _currentProject.GetLastBuildTime());
                Schemas.ItemsSource = _currentProject.GetSchemas();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error see output window "; });

                OutputWindowMessage.WriteMessage("Error reading project: {0}", ex.Message);
            }
        }

        private void Schemas_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (null == Schemas.SelectedValue)
                    return;

                var schemaName = Schemas.SelectedValue.ToString();

                if (String.IsNullOrEmpty(schemaName))
                    return;

                _currentSchema = _currentProject.GetSchema(schemaName);

                Tables.ItemsSource = _currentSchema.GetTables();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error see output window"; });

                OutputWindowMessage.WriteMessage("Error selecting schema:");
                OutputWindowMessage.WriteMessage(ex.Message);
            }
        }

        private void Tables_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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

                DataGrid.DataContext = _currentTable.Data.DefaultView;
                //TODO -= check for null and start adding a datatable when building the table (maybe need a lazy loading)
                //we also need a repository of merge statements which is the on disk representation so we can grab those
                //if they exist or just create a new one - then save them back and 
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error Enumerating projects: " + ex.Message; });

                OutputWindowMessage.WriteMessage("Error selecting table ({0}-):",
                    _currentTable == null ? "null" : _currentTable.Name,
                    Tables.SelectedValue == null ? "selected = null" : Tables.SelectedValue.ToString());
                OutputWindowMessage.WriteMessage(ex.Message);
            }
        }

        private bool CheckSaveChanges()
        {
            return true;
        }

        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            _currentDataGridDirty = true;
        }

        private void button1_Save(object sender, RoutedEventArgs e)
        {
            //need to finish off saving back to the files (need a radio button with pre/post deploy (not changeable when read from file) - futrue feature
            //need a check to write files on window closing
            //need lots of tests
            try
            {
                _solution.Save();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error see output window"; });
                
                OutputWindowMessage.WriteMessage("Error saving solution files:");
                OutputWindowMessage.WriteMessage(ex.Message);
            }
        }

        private void ImportTable(object sender, RoutedEventArgs e)
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
                Dispatcher.Invoke(() => { LastStatusMessage.Text = "Error see output window"; });


                OutputWindowMessage.WriteMessage("Error importing data (table={0}):", _currentTable.Name);
                OutputWindowMessage.WriteMessage(ex.Message);
            }

            DataGrid.DataContext = _currentTable.Data.DefaultView;
        }

        private void Logging_OnChecked(object sender, RoutedEventArgs e)
        {
            DebugLogging.Enable = Logging.IsChecked.Value;
        }
    }

    public interface IStatus
    {
        void SetStatus(string message);
    }
}