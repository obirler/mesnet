/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/
using System.Windows.Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for RightFixedSupportItem.xaml
    /// </summary>
    public partial class RightFixedSupportItem : UserControl
    {
        public RightFixedSupportItem(RightFixedSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = GetString("rightfixedsupport") + " " + Support.SupportId;
        }

        public RightFixedSupport Support;
    }
}
