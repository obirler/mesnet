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
using Mesnet.Classes.Math;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for ConcentratedLoadPrompt.xaml
    /// </summary>
    public partial class ConcentratedLoadPrompt : Window
    {
        public ConcentratedLoadPrompt(Beam beam)
        {
            InitializeComponent();

            _length = beam.Length;

            _loads = new KeyValueCollection();

            if (beam.ConcentratedLoads?.Count > 0)
            {
                foreach (KeyValuePair<double, double> pair in beam.ConcentratedLoads)
                {                   
                    var fnc = new ConcentratedLoadFunction();
                    fnc.function.Text = "P = " + pair.Value + " kN";
                    fnc.limits.Text = "x = " + pair.Key + " m";
                    fnc.removebtn.Click += Remove_Click;
                    fncstk.Children.Add(fnc);
                    _loads.Add(pair.Key, pair.Value);
                }
            }

            loadx.Text = (_length/2).ToString();
        }

        private double _length;

        private KeyValueCollection _loads;

        public KeyValueCollection Loads
        {
            get { return _loads; }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            double x = Convert.ToDouble(loadx.Text);

            if (double.TryParse(loadx.Text, out x))
            {
                if (x < 0 || x >= _length)
                {
                    MessageBox.Show(GetString("outrange"));
                    loadx.Focus();
                    return;
                }
                
                foreach (KeyValuePair<double, double> load in _loads)
                {
                    if (load.Key == x)
                    {
                        MessageBox.Show(GetString("invalidpoint"));
                        loadx.Focus();
                        return;
                    }
                }        
            }
            else
            {
                MessageBox.Show(GetString("invalidvalue"));
                loadx.Focus();
                return;
            }


            double y;

            if (double.TryParse(load.Text, out y))
            {
                if (y == 0)
                {
                    MessageBox.Show(GetString("invalidconcload"));
                    load.Focus();
                    return;                    
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidvalue"));
                load.Focus();
                return;
            }

            _loads.Add(x, y);

            var fnc = new ConcentratedLoadFunction();
            fnc.function.Text = "P = " + y + " kN";
            fnc.limits.Text = "x = " + x + " m";
            fnc.removebtn.Click += Remove_Click;

            fncstk.Children.Add(fnc);

            load.Text = "10";
            loadx.Text = (_length/2).ToString();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var stk = (sender as Button).Parent as StackPanel;
            var fnc = stk.Parent as ConcentratedLoadFunction;
            var index = fncstk.Children.IndexOf(fnc);
            _loads.RemoveAt(index);
            fncstk.Children.RemoveAt(index);
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
