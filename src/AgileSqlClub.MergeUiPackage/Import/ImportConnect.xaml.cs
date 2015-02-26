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

namespace AgileSqlClub.MergeUi.Import
{
    /// <summary>
    /// Interaction logic for ImportConnect.xaml
    /// </summary>
    public partial class ImportConnect : Page
    {
        public ImportConnect()
        {
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
            throw new NotImplementedException();
        }
    }
}
