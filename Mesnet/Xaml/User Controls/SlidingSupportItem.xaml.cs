using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for SlidingSupportItem.xaml
    /// </summary>
    public partial class SlidingSupportItem : UserControl
    {
        public SlidingSupportItem(string supportname)
        {
            InitializeComponent();
            support.Text = supportname;
        }
    }
}
