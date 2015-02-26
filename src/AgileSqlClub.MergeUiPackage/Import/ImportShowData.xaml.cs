using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using AgileSqlClub.MergeUi.Metadata;

namespace AgileSqlClub.MergeUi.Import
{
    /// <summary>
    /// Interaction logic for ImportShowData.xaml
    /// </summary>
    public partial class ImportShowData : Page, IMovingPage
    {
        private readonly ImportData _importData;
        private readonly ITable _table;
        
        public ImportShowData(ImportData importData, ITable table)
        {
            InitializeComponent();
            _importData = importData;
            _table = table;
        }

        public void SaveData()
        {
            
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => ImportTable());
        }

        private void ImportTable()
        {
            try
            {
                using (var con = new SqlConnection(_importData.ConnectionString + "initial catalog=" + _importData.Database))
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = string.Format("select * from [{0}].[{1}]", _table.SchemaName, _table.Name);
                        var dataTable = new DataTable();
                        dataTable.Load(cmd.ExecuteReader());
                        _importData.Table = dataTable;

                        _importData.Dispatcher.Invoke(() =>
                        {
                            DataGrid.DataContext = _importData.Table.DefaultView;
                        });
                    }
                } 
            }
            catch (Exception e)
            {
                this.Dispatcher.Invoke(() => MessageBox.Show("Error importing data: " + e.Message));
            }
        }
    }
}
