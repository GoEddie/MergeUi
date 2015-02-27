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

namespace AgileSqlClub.MergeUi.Import
{
    /// <summary>
    /// Interaction logic for ImportConnect.xaml
    /// </summary>
    public partial class ImportConnect : Page, IMovingPage
    {
        private readonly ImportData _importData;

        public ImportConnect(ImportData importData)
        {
            _importData = importData;
            InitializeComponent();
        }

        private void WinAuth_OnClick(object sender, RoutedEventArgs e)
        {
            User.IsEnabled = false;
            Pass.IsEnabled = false;
        }

        private void SqlAuth_OnClick(object sender, RoutedEventArgs e)
        {
            User.IsEnabled = true;
            Pass.IsEnabled = true;
        }

        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                GetDatabaseList(GetConnectionString());
            });
        }

        private string GetConnectionString()
        {

            
            return Dispatcher.Invoke(() =>
            {
                return string.Format("server={0};{1}", Server.Text,
                    (WinAuth.IsChecked.Value
                        ? "Integrated Security=SSPI;"
                        : string.Format("UID={0};PWD={1};", User.Text, Pass.Password)));
            }) as string;
            
        }

        private void GetDatabaseList(string connectionString)
        {
            try
            {
                this.Dispatcher.Invoke(() => Connect.IsEnabled = false);

                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select name from sys.databases order by name";
                        var list = new List<string>();
                        var sdr = cmd.ExecuteReader();
                        while (sdr.Read())
                        {
                            list.Add(sdr[0].ToString());
                        }
                        _importData.ConnectionString = connectionString;
                        
                        this.Dispatcher.Invoke(() => {
                                                         DatabaseList.ItemsSource = list;
                        });
                    }
                }
            }
            catch (Exception e)
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Error - unable to get database list, error: " + e.Message);
                });
            }

            this.Dispatcher.Invoke(() => Connect.IsEnabled = true);
        }

        public void ShowData()
        {
            
        }

        public void SaveData()
        {
            _importData.Database = DatabaseList.SelectedValue == null ? null : DatabaseList.SelectedValue.ToString();
        }
    }

    public interface IMovingPage
    {
        void ShowData();
        void SaveData();
    }
}
