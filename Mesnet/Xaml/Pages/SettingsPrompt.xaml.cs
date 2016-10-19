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
        }

        private void englishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("en-EN");
        }

        private void turkishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("tr-TR");
        }
    }
}
