using System.Windows.Controls;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BeamItem.xaml
    /// </summary>
    public partial class BeamItem : UserControl
    {
        public BeamItem(string name)
        {
            InitializeComponent();
            beamname.Text = name;
        }
    }
}
