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
using Mesnet.Properties;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPrompt.xaml
    /// </summary>
    public partial class SettingsPrompt : Window
    {
        public SettingsPrompt()
        {
            InitializeComponent();

            switch (Settings.Default.language)
            {
                case "en-EN":

                    englishbtn.IsChecked = true;

                    break;

                case "tr-TR":

                    turkishbtn.IsChecked = true;

                    break;
            }

            switch (Calculation)
            {
                case CalculationType.SingleThreaded:

                    calculationcbx.SelectedIndex = 0;

                    break;

                case CalculationType.MultiThreaded:

                    calculationcbx.SelectedIndex = 1;

                    break;
            }
        }

        private void englishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("en-EN");
        }

        private void turkishbtn_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("tr-TR");
        }

        private void calculationcbx_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (calculationcbx.SelectedIndex)
            {
                case 0:

                    Calculation = CalculationType.SingleThreaded;

                    break;

                case 1:

                    Calculation = CalculationType.MultiThreaded;

                    break;
            }
        }
    }
}
