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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mesnet.Classes.Math;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for DistributedLoadPrompt.xaml
    /// </summary>
    public partial class DistributedLoadPrompt : Window
    {
        public DistributedLoadPrompt(Beam beam)
        {
            InitializeComponent();

            BeamLength = beam.Length;

            Loadpolies = new List<Poly>();

            if (beam.DistributedLoads.Count > 0)
            {
                foreach (Poly loadpoly in beam.DistributedLoads.PolyList())
                {
                    Loadpolies.Add(loadpoly);
                    var fnc = new LoadFunction();
                    fnc.function.Text = "q(x) = " + loadpoly.ToString();
                    fnc.limits.Text = loadpoly.StartPoint + " <= x <= " + loadpoly.EndPoint;
                    fnc.removebtn.Click += Remove_Click;
                    fncstk.Children.Add(fnc);
                }

                finishbtn.Visibility = Visibility.Visible;
            }
        }

        public List<Poly> Loadpolies;

        public double BeamLength;

        private void udlbtn_Click(object sender, RoutedEventArgs e)
        {
            double startp;
            if (double.TryParse(udlx1.Text, out startp))
            {
                if (startp < 0 || startp >= BeamLength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    udlx1.Focus();
                    return;
                }
                if (Loadpolies.Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    udlx1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                udlx1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(udlx2.Text, out endp))
            {
                if (endp > BeamLength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    udlx2.Focus();
                    return;
                }
                if (Loadpolies.Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    udlx2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                udlx2.Focus();
                return;
            }

            double load;
            if (!double.TryParse(ldlload1.Text, out load))
            {
                MessageBox.Show(GetString("invalidvalue"));
                udlload.Focus();
                return;
            }

            var poly = new Poly(udlload.Text);
            poly.StartPoint = Convert.ToDouble(udlx1.Text);
            poly.EndPoint = Convert.ToDouble(udlx2.Text);

            Loadpolies.Add(poly);

            var fnc = new LoadFunction();
            fnc.function.Text = "q(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void ldlbtn_Click(object sender, RoutedEventArgs e)
        {
            double startp;
            if (double.TryParse(ldlx1.Text, out startp))
            {
                if (startp < 0 || startp >= BeamLength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    ldlx1.Focus();
                    return;
                }
                if (Loadpolies.Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    ldlx1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                ldlx1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(ldlx2.Text, out endp))
            {
                if (endp > BeamLength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    ldlx2.Focus();
                    return;
                }
                if (Loadpolies.Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    ldlx2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                ldlx2.Focus();
                return;
            }

            double startload;
            if (!double.TryParse(ldlload1.Text, out startload))
            {
                MessageBox.Show(GetString("invalidvalue"));
                ldlload1.Focus();
                return;
            }

            double endload;
            if (!double.TryParse(ldlload2.Text, out endload))
            {
                MessageBox.Show(GetString("invalidvalue"));
                ldlload2.Focus();
                return;
            }

            var m = (Convert.ToDouble(ldlload2.Text) - Convert.ToDouble(ldlload1.Text)) /
                    (Convert.ToDouble(ldlx2.Text) - Convert.ToDouble(ldlx1.Text));

            var prepoly = new Poly();

            var c = -m * Convert.ToDouble(ldlx2.Text);
            if (c != 0)
            {
                try
                {
                    prepoly = prepoly + new Poly(c.ToString());
                }
                catch (NullReferenceException)
                {
                    prepoly = new Poly(c.ToString());
                }

            }
            var d = Convert.ToDouble(ldlload2.Text);

            if (d != 0)
            {
                try
                {
                    prepoly = prepoly + new Poly(d.ToString());
                }
                catch (Exception)
                {
                    prepoly = new Poly(d.ToString());
                }

            }
            if (m != 0)
            {
                try
                {
                    prepoly = prepoly + (new Poly(m.ToString())) * (new Poly("x"));
                }
                catch (Exception)
                {
                    prepoly = (new Poly(m.ToString())) * (new Poly("x"));
                }
            }

            prepoly.StartPoint = Convert.ToDouble(ldlx1.Text);
            prepoly.EndPoint = Convert.ToDouble(ldlx2.Text);
            Loadpolies.Add(prepoly);

            var fnc = new LoadFunction();
            fnc.function.Text = "q(x) = " + prepoly.ToString();
            fnc.limits.Text = prepoly.StartPoint + " <= x <= " + prepoly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void vdlbtn_Click(object sender, RoutedEventArgs e)
        {
            double startp;
            if (double.TryParse(vdlx1.Text, out startp))
            {
                if (startp < 0 || startp >= BeamLength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    vdlx1.Focus();
                    return;
                }
                if (Loadpolies.Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    vdlx1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                vdlx1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(vdlx2.Text, out endp))
            {
                if (endp > BeamLength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    vdlx2.Focus();
                    return;
                }
                if (Loadpolies.Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    vdlx2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                vdlx2.Focus();
                return;
            }

            if (!Poly.ValidateExpression(vdlload.Text))
            {
                MessageBox.Show(GetString("invalidpolynomial"));
                vdlload.Focus();
                return;
            }

            var poly = new Poly(vdlload.Text);
            poly.StartPoint = Convert.ToDouble(vdlx1.Text);
            poly.EndPoint = Convert.ToDouble(vdlx2.Text);

            Loadpolies.Add(poly);

            var fnc = new LoadFunction();
            fnc.function.Text = "q(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var stk = (sender as Button).Parent as StackPanel;
            var fnc = stk.Parent as LoadFunction;
            var index = fncstk.Children.IndexOf(fnc);
            Loadpolies.RemoveAt(index);
            fncstk.Children.RemoveAt(index);

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Collapsed;
            }
        }

        private void resetpanel()
        {
            udlload.IsEnabled = false;
            udlload.Text = "";
            udlx1.IsEnabled = false;
            udlx1.Text = "";
            udlx2.IsEnabled = false;
            udlx2.Text = "";
            udlbtn.IsEnabled = false;

            ldlload1.IsEnabled = false;
            ldlload1.Text = "";
            ldlload2.IsEnabled = false;
            ldlload2.Text = "";
            ldlx1.IsEnabled = false;
            ldlx1.Text = "";
            ldlx2.IsEnabled = false;
            ldlx2.Text = "";
            ldlbtn.IsEnabled = false;

            vdlload.IsEnabled = false;
            vdlload.Text = "";
            vdlx1.IsEnabled = false;
            vdlx1.Text = "";
            vdlx2.IsEnabled = false;
            vdlx2.Text = "";
            vdlbtn.IsEnabled = false;

            udlexpand.IsExpanded = false;
            ldlexpand.IsExpanded = false;
            vdlexpand.IsExpanded = false;
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void udlexpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (Loadpolies.Count == 0)
            {
                udlload.Text = "10";
                udlx1.Text = "0";
                udlx2.Text = BeamLength.ToString();
            }
            udlload.IsEnabled = true;
            udlx1.IsEnabled = true;
            udlx2.IsEnabled = true;
            udlbtn.IsEnabled = true;

            ldlload1.IsEnabled = false;
            ldlload1.Text = "";
            ldlload2.IsEnabled = false;
            ldlload2.Text = "";
            ldlx1.IsEnabled = false;
            ldlx1.Text = "";
            ldlx2.IsEnabled = false;
            ldlx2.Text = "";
            ldlbtn.IsEnabled = false;

            vdlload.IsEnabled = false;
            vdlload.Text = "";
            vdlx1.IsEnabled = false;
            vdlx1.Text = "";
            vdlx2.IsEnabled = false;
            vdlx2.Text = "";
            vdlbtn.IsEnabled = false;

            ldlexpand.IsExpanded = false;
            vdlexpand.IsExpanded = false;
        }

        private void ldlexpand_Expanded(object sender, RoutedEventArgs e)
        {
            ldlload1.IsEnabled = true;
            ldlload2.IsEnabled = true;
            ldlx1.IsEnabled = true;
            ldlx2.IsEnabled = true;
            ldlbtn.IsEnabled = true;

            udlload.IsEnabled = false;
            udlload.Text = "";
            udlx1.IsEnabled = false;
            udlx1.Text = "";
            udlx2.IsEnabled = false;
            udlx2.Text = "";
            udlbtn.IsEnabled = false;

            vdlload.IsEnabled = false;
            vdlload.Text = "";
            vdlx1.IsEnabled = false;
            vdlx1.Text = "";
            vdlx2.IsEnabled = false;
            vdlx2.Text = "";
            vdlbtn.IsEnabled = false;

            udlexpand.IsExpanded = false;
            vdlexpand.IsExpanded = false;
        }

        private void vdlexpand_Expanded(object sender, RoutedEventArgs e)
        {
            vdlload.IsEnabled = true;
            vdlx1.IsEnabled = true;
            vdlx2.IsEnabled = true;
            vdlbtn.IsEnabled = true;

            udlload.IsEnabled = false;
            udlload.Text = "";
            udlx1.IsEnabled = false;
            udlx1.Text = "";
            udlx2.IsEnabled = false;
            udlx2.Text = "";
            udlbtn.IsEnabled = false;

            ldlload1.IsEnabled = false;
            ldlload1.Text = "";
            ldlload2.IsEnabled = false;
            ldlload2.Text = "";
            ldlx1.IsEnabled = false;
            ldlx1.Text = "";
            ldlx2.IsEnabled = false;
            ldlx2.Text = "";
            ldlbtn.IsEnabled = false;

            udlexpand.IsExpanded = false;
            ldlexpand.IsExpanded = false;
        }
    }
}
