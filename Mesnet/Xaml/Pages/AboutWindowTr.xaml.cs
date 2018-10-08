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

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for AboutWindowTr.xaml
    /// </summary>
    public partial class AboutWindowTr : Window
    {
        public AboutWindowTr()
        {
            InitializeComponent();

            versiontext.Text = "V " + Config.VersionNumber;
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void developerpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.linkedin.com/in/ömer-birler-9582696b");
            e.Handled = true;
        }

        private void instructorpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://knot.gidb.itu.edu.tr/gemi/personel/bayraktarkatal.html");
            e.Handled = true;
        }

        private void mailtodeveloper_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:omer.birler@gmail.com");
            e.Handled = true;
        }

        private void sourcecodepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitbucket.org/omerbirler/mesnet");
            e.Handled = true;
        }

        private void gpllicencepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses");
            e.Handled = true;
        }
    }
}
