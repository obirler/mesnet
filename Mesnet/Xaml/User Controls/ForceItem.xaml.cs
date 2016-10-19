using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ForceItem.xaml
    /// </summary>
    public partial class ForceItem : UserControl
    {
        public ForceItem(string name)
        {
            InitializeComponent();
            force.Text = name;
        }
    }
}
