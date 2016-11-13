using System.Windows;
using Mesnet.Properties;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPrompt.xaml
    /// </summary>
    public partial class SettingsPrompt : Window
    {
        public SettingsPrompt()
        {
            InitializeComponent();

            switch (Settings.Default.language)
            {
                case "en-EN":

                    englishbtn.IsChecked = true;

                    break;

                case "tr-TR":

                    turkishbtn.IsChecked = true;

                    break;
            }

            switch (Calculation)
            {
                case CalculationType.SingleThreaded:

                    calculationcbx.SelectedIndex = 0;

                    break;

                case CalculationType.MultiThreaded:

                    calculationcbx.SelectedIndex = 1;

                    break;
            }
        }

        private void englishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("en-EN");
        }

        private void turkishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("tr-TR");
        }

        private void calculationcbx_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (calculationcbx.SelectedIndex)
            {
                case 0:

                    Calculation = CalculationType.SingleThreaded;

                    break;

                case 1:

                    Calculation = CalculationType.MultiThreaded;

                    break;
            }
        }
    }
}
