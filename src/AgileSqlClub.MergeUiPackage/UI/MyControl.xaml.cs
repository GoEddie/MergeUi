using System.Windows;
using System.Windows.Controls;
using AgileSqlClub.MergeUi.VSServices;

namespace AgileSqlClub.MergeUi.UI
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {

            var enumerator = new ProjectEnumerator();
            enumerator.EnumerateProjects();

            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "MergeUi");

        }
    }
}