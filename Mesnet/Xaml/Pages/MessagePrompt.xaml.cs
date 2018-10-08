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

using Mesnet.Classes;
using System.Windows;
using System.Windows.Input;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for MessagePrompt.xaml
    /// </summary>
    public partial class MessagePrompt : Window
    {
        public MessagePrompt(string message)
        {
            InitializeComponent();
            Message.Text = message;
        }

        public Global.DialogResult Result = Global.DialogResult.None;

        private void yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Yes;
            DialogResult = true;
        }

        private void nobtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.No;
            DialogResult = true;
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Cancel;
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Result = Global.DialogResult.Yes;
                DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                Result = Global.DialogResult.No;
                DialogResult = true;
            }
        }
    }
}
