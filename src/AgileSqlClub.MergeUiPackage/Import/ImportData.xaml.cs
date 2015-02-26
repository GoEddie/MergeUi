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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace AgileSqlClub.MergeUi.Import
{
    /// <summary>
    /// Interaction logic for ImportData.xaml
    /// </summary>
    public partial class ImportData
    {
        public ImportData()
        {
            InitializeComponent();
            _pages = new List<Page>() { new ImportConnect(), new ImportShowData() , new ImportConfirm()};
            Transitioning.Content = _pages[0].Content;
        }

        private List<Page> _pages;
        private int _currentPage = 0;

        private void NextControl(object sender, RoutedEventArgs e)
        {
            if (_currentPage >= _pages.Count)
                return;

            Transitioning.Content = _pages[_currentPage++].Content;
            if (_currentPage >= _pages.Count)
                return;

            Forward.IsEnabled = false;
        }
    }
}
