using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for InertiaItem.xaml
    /// </summary>
    public partial class InertiaItem : UserControl
    {
        public InertiaItem(string name)
        {
            InitializeComponent();
            inertiatext.Text = name;
        }
    }
}
