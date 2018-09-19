using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mesnet.Classes;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for AboutWindowTr.xaml
    /// </summary>
    public partial class AboutWindowTr : Window
    {
        public AboutWindowTr()
        {
            InitializeComponent();

            versiontext.Text = "V " + Global.VersionNumber;
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void developerpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.linkedin.com/in/ömer-birler-9582696b");
            e.Handled = true;
        }

        private void instructorpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://knot.gidb.itu.edu.tr/gemi/personel/bayraktarkatal.html");
            e.Handled = true;
        }

        private void mailtodeveloper_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:omer.birler@gmail.com");
            e.Handled = true;
        }

        private void sourcecodepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitbucket.org/omerbirler/mesnet");
            e.Handled = true;
        }

        private void gpllicencepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses");
            e.Handled = true;
        }
    }
}
