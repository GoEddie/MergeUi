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
using MahApps.Metro.Controls;

namespace AgileSqlClub.MergeUI.UI
{
    /// <summary>
    /// Interaction logic for MergeUi.xaml
    /// </summary>
    public partial class MergeUi : UserControl, IStatus
    {
        public MergeUi()
        {
            InitializeComponent();
        }

        public void SetStatus(string message)
        {
            
        }
    }
}
