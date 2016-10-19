using System.Windows;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for RotatePrompt.xaml
    /// </summary>
    public partial class RotatePrompt : Window
    {
        public RotatePrompt()
        {
            InitializeComponent();
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
