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

using System.Windows;
using System.Windows.Input;
using Mesnet.Classes;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for NewVersionPrompt.xaml
    /// </summary>
    public partial class NewVersionPrompt : Window
    {
        public NewVersionPrompt(string versionnumber)
        {
            InitializeComponent();
            version.Text = versionnumber;
        }

        public Global.DialogResult Result = Global.DialogResult.None;

        public bool DontAskToUpdate = false;

        private void yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Yes;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void nobtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.No;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Cancel;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Result = Global.DialogResult.Yes;
                DontAskToUpdate = (bool)dontshowcbx.IsChecked;
                DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                Result = Global.DialogResult.No;
                DontAskToUpdate = (bool)dontshowcbx.IsChecked;
                DialogResult = true;
            }
        }
    }
}
