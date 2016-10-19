using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for MomentItem.xaml
    /// </summary>
    public partial class MomentItem : UserControl
    {
        public MomentItem(string name)
        {
            InitializeComponent();
            moment.Text = name;
        }
    }
}
