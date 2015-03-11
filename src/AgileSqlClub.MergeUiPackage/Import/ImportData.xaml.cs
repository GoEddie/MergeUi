using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using AgileSqlClub.MergeUI.Extensions;
using AgileSqlClub.MergeUI.Metadata;
using MahApps.Metro.Controls;

namespace AgileSqlClub.MergeUI.Import
{
    /// <summary>
    /// Interaction logic for ImportData.xaml
    /// </summary>
    public partial class ImportData
    {
        private readonly ITable _table;

        public ImportData(ITable table)
        {
            _table = table;
            InitializeComponent();
            _pages = new List<Page>() { new ImportConnect(this), new ImportShowData(this, _table)};
            Transitioning.Content = _pages[0].Content;
        }

        private List<Page> _pages;
        private int _currentPage = 0;

        private void NextControl(object sender, RoutedEventArgs e)
        {
            if (Forward.Content == "Import")
            {
                Save();
                return;
            }

            if (_currentPage > _pages.Count)
                return;

            Back.IsEnabled = true;

            (_pages[_currentPage] as IMovingPage).SaveData();

            Transitioning.Content = _pages[++_currentPage].Content;

            (_pages[_currentPage] as IMovingPage).ShowData();

            if (_currentPage > _pages.Count)
                return;
            
            Forward.Content = "Import";
        }

        private void Save()
        {
            _table.Data = Table;
            _table.Data.SetDirty();
            
            Close();
        }

        private void PreviousControl(object sender, RoutedEventArgs e)
        {
            if (_currentPage <= 0)
                return;

            Forward.IsEnabled = true;
            Forward.Content = "Next";

            Transitioning.Content = _pages[--_currentPage].Content;

            if (_currentPage > 0)
                return;

            
            Back.IsEnabled = false;
        }

        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public DataTable Table { get; set; }
    }
}
