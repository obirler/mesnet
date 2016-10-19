using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for StressItem.xaml
    /// </summary>
    public partial class StressItem : UserControl
    {
        public StressItem(string name)
        {
            InitializeComponent();

            stress.Text = name;
        }
    }
}
