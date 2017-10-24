using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BeamSupportItem.xaml
    /// </summary>
    public partial class BeamSupportItem : UserControl
    {
        public BeamSupportItem(string header, object support)
        {
            InitializeComponent();
            this.Header.Text = header;
            Support = support;
        }

        public BeamSupportItem()
        {
            InitializeComponent();
        }

        public object Support = null;
    }
}
