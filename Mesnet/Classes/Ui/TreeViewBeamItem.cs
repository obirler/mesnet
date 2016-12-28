using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Mesnet.Xaml.User_Controls;

namespace Mesnet.Classes.Ui
{
    public class TreeViewBeamItem : TreeViewItem
    {
        public TreeViewBeamItem(Beam beam)
        {
            _beam = beam;
        }

        private Beam _beam;

        public Beam Beam
        {
            get { return _beam; }
        }
    }
}
