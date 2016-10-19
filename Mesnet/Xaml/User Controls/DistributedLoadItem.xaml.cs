using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for DistributedLoadItem.xaml
    /// </summary>
    public partial class DistributedLoadItem : UserControl
    {
        public DistributedLoadItem(string name)
        {
            InitializeComponent();
            distload.Text = name;
        }
    }
}
