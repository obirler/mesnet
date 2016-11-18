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
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mesnet.Xaml.User_Controls;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for ConcentratedLoadPrompt.xaml
    /// </summary>
    public partial class ConcentratedLoadPrompt : Window
    {
        public ConcentratedLoadPrompt(double length)
        {
            InitializeComponent();

            _length = length;

            _loads = new List<KeyValuePair<double, double>>();
        }

        private double _length;

        private List<KeyValuePair<double, double>> _loads;

        public List<KeyValuePair<double, double>> Loads
        {
            get { return _loads; }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            double x = Convert.ToDouble(loadx.Text);
            double y = Convert.ToDouble(load.Text);

            _loads.Add(new KeyValuePair<double, double>(x, y));

            var fnc = new ConcentratedLoadFunction();
            fnc.function.Text = "P = " + y + " kN";
            fnc.limits.Text = "x = " + x + " m";
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            load.Text = "";
            loadx.Text = "";
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var stk = (sender as Button).Parent as StackPanel;
            var fnc = stk.Parent as LoadFunction;
            var index = fncstk.Children.IndexOf(fnc);
            _loads.RemoveAt(index);
            fncstk.Children.RemoveAt(index);

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Collapsed;
            }
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
