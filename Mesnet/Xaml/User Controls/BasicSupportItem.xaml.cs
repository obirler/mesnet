using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BasicSupportItem.xaml
    /// </summary>
    public partial class BasicSupportItem : UserControl
    {
        public BasicSupportItem(string name)
        {
            InitializeComponent();
            support.Text = name;
        }
    }
}