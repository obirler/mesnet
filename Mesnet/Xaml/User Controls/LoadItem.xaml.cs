using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LoadItem.xaml
    /// </summary>
    public partial class LoadItem : UserControl
    {
        public LoadItem(string loadname)
        {
            InitializeComponent();
            load.Text = loadname;
        }
    }
}
