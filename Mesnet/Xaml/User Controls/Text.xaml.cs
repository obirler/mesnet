using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Text.xaml
    /// </summary>
    public partial class Text : UserControl
    {
        public Text(string text)
        {
            InitializeComponent();

            tooltip.Text = text;

            tooltip.Width = tooltip.ActualWidth;
            tooltip.Height = tooltip.ActualHeight;
        }
    }
}
