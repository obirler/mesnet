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
