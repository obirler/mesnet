using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LeftFixedSupportItem.xaml
    /// </summary>
    public partial class LeftFixedSupportItem : UserControl
    {
        public LeftFixedSupportItem(string name)
        {
            InitializeComponent();
            support.Text = name;
        }
    }
}
