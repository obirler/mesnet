using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ButtonItem.xaml
    /// </summary>
    public partial class ButtonItem : UserControl
    {
        public ButtonItem(string name)
        {
            InitializeComponent();
            content.Content = name;
        }
    }
}
