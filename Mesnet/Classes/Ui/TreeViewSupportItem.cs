using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Mesnet.Classes.Ui
{
    public class TreeViewSupportItem:TreeViewItem
    {
        public TreeViewSupportItem(object support)
        {
            _support = support;
        }

        private object _support;

        public object Support
        {
            get { return _support; }
        }
    }
}
