using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for RightFixedSupportItem.xaml
    /// </summary>
    public partial class RightFixedSupportItem : UserControl
    {
        public RightFixedSupportItem(string name)
        {
            InitializeComponent();
            support.Text = name;
        }
    }
}
