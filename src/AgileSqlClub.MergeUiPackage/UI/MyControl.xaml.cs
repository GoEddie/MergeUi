using System;
using System.Web.UI.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;
using AgileSqlClub.MergeUi.VSServices;
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
            Projects.ItemsSource = null;
            Schemas.ItemsSource = null;
            Tables.ItemsSource = null;

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

            DataGrid.DataContext = _currentTable.Data.DefaultView;
        }
    }
}