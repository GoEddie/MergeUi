using System.Windows;
using System.Windows.Controls;
using AgileSqlClub.MergeUi.DacServices;
using AgileSqlClub.MergeUi.Metadata;
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

            var solution = new Solution(new ProjectEnumerator(), new DacParserBuilder());
            solution.GetProject("erm??");
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "MergeUi");

        }
    }
}