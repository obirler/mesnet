using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LengthItem.xaml
    /// </summary>
    public partial class LengthItem : UserControl
    {
        public LengthItem(string name)
        {
            InitializeComponent();
            length.Text = name;
        }
    }
}
