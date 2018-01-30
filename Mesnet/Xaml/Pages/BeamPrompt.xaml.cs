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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.Math;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for BeamPrompt.xaml
    /// </summary>
    public partial class BeamPrompt : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeamPrompt"/> class. Used when user pressed the beam button. User adds a new beam.
        /// </summary>
        public BeamPrompt()
        {
            InitializeComponent();

            inertiappoly = new PiecewisePoly();

            _loaded = true;

            length.Focus();

            if (inertiappoly.Length == 0)
            {
                ui.Text = "1";
                uix1.Text = "0";
                uix2.Text = beamlength.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeamPrompt"/> class. Adds a beam between start and end points. The length and angle of the beam predefined by the points.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public BeamPrompt(Point start, Point end)
        {
            InitializeComponent();

            inertiappoly = new PiecewisePoly();
            beamlength = Math.Round(Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2)) / 100, 4);
            length.Text = beamlength.ToString();
            length.IsEnabled = false;
            angletbx.Text = getAngle(start, end).ToString();
            angletbx.IsEnabled = false;
            _loaded = true;

            if (inertiappoly.Length == 0)
            {
                ui.Text = "1";
                uix1.Text = "0";
                uix2.Text = beamlength.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeamPrompt"/> class.
        /// </summary>
        /// <param name="beam">The beam to be edited.</param>
        /// <param name="canbeedited">if set to false the length and the angle of the beam can not be edited..</param>
        public BeamPrompt(Beam beam, bool canbeedited)
        {
            InitializeComponent();

            Title = GetString("editbeam");

            _existingbeam = beam;

            beamlength = _existingbeam.Length;
            length.Text = beamlength.ToString();
            angletbx.Text = _existingbeam.Angle.ToString();

            if (!canbeedited)
            {
                length.IsEnabled = false;
                angletbx.IsEnabled = false;
            }

            elasticitymodulus.Text = _existingbeam.ElasticityModulus.ToString();
            stresscbx.IsChecked = _existingbeam.PerformStressAnalysis;

            inertiappoly = new PiecewisePoly();

            foreach (Poly poly in _existingbeam.Inertias)
            {
                var ineritiapoly = new Poly(poly.ToString());
                ineritiapoly.StartPoint = Convert.ToDouble(poly.StartPoint);
                ineritiapoly.EndPoint = Convert.ToDouble(poly.EndPoint);
                inertiappoly.Add(ineritiapoly);
            }

            if (_existingbeam.PerformStressAnalysis)
            {
                eppoly = new PiecewisePoly();

                foreach (Poly poly in _existingbeam.E)
                {
                    var epoly = new Poly(poly.ToString());
                    epoly.StartPoint = Convert.ToDouble(poly.StartPoint);
                    epoly.EndPoint = Convert.ToDouble(poly.EndPoint);
                    eppoly.Add(epoly);
                }

                dppoly = new PiecewisePoly();

                foreach (Poly poly in _existingbeam.D)
                {
                    var dpoly = new Poly(poly.ToString());
                    dpoly.StartPoint = Convert.ToDouble(poly.StartPoint);
                    dpoly.EndPoint = Convert.ToDouble(poly.EndPoint);
                    dppoly.Add(dpoly);
                }
            }

            updatefncstk();

            finishbtn.Visibility = Visibility.Visible;
        }

        public double beamlength = 0;

        public double beamelasticitymodulus = 0;

        public double beaminertiamodulus = 0;

        public double angle = 0;

        public PiecewisePoly inertiappoly;

        public PiecewisePoly eppoly;

        public PiecewisePoly dppoly;

        public FunctionType type;

        private bool _loaded = false;

        private readonly Beam _existingbeam = null;

        private void length_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (length.Text != "")
                {
                    beamlength = Convert.ToDouble(length.Text);
                    length.Background = new SolidColorBrush(Color.FromArgb(200, 48, 247, 66));
                    uix2.Text = length.Text;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(FindResource("invalidbeamlength").ToString());
                length.Background = new SolidColorBrush(Color.FromArgb(200, 255, 97, 97));
            }
        }

        private void elasticitymodulus_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (elasticitymodulus.Text != "")
                {
                    beamelasticitymodulus = Convert.ToDouble(elasticitymodulus.Text);
                    elasticitymodulus.Background = new SolidColorBrush(Color.FromArgb(200, 48, 247, 66));
                }
            }
            catch (Exception)
            {
                elasticitymodulus.Background = new SolidColorBrush(Color.FromArgb(200, 255, 97, 97));
            }
        }

        private void angletbx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (angletbx.Text != "")
                {
                    angle = Convert.ToDouble(angletbx.Text);
                    angletbx.Background = new SolidColorBrush(Color.FromArgb(200, 48, 247, 66));
                }
            }
            catch (Exception)
            {
                angletbx.Background = new SolidColorBrush(Color.FromArgb(200, 255, 97, 97));
            }
        }

        private void uibtn_Click(object sender, RoutedEventArgs e)
        {
            stresscbx.IsEnabled = false;

            double inert;
            if (double.TryParse(ui.Text, out inert))
            {
                if (inert <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    ui.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                ui.Focus();
                return;
            }

            double startp;
            if (double.TryParse(uix1.Text, out startp))
            {
                if (startp < 0 || startp >= beamlength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    uix1.Focus();
                    return;
                }
                if (inertiappoly.Cast<Poly>().Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    uix1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                uix1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(uix2.Text, out endp))
            {
                if (endp > beamlength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    uix2.Focus();
                    return;
                }

                if (inertiappoly.Cast<Poly>().Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    uix2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                uix2.Focus();
                return;
            }

            var ineritiapoly = new Poly(ui.Text);
            ineritiapoly.StartPoint = Convert.ToDouble(uix1.Text);
            ineritiapoly.EndPoint = Convert.ToDouble(uix2.Text);

            inertiappoly.Add(ineritiapoly);

            if ((bool)stresscbx.IsChecked)
            {
                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }
                var epoly = new Poly(eui.Text);
                epoly.StartPoint = Convert.ToDouble(uix1.Text);
                epoly.EndPoint = Convert.ToDouble(uix2.Text);
                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }
                var dpoly = new Poly(dui.Text);
                dpoly.StartPoint = Convert.ToDouble(uix1.Text);
                dpoly.EndPoint = Convert.ToDouble(uix2.Text);
                dppoly.Add(dpoly);
            }

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + ineritiapoly.ToString();
            fnc.limits.Text = ineritiapoly.StartPoint + " <= x <= " + ineritiapoly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void libtn_Click(object sender, RoutedEventArgs e)
        {
            double inerts;
            if (double.TryParse(li1.Text, out inerts))
            {
                if (inerts <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    li1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                li1.Focus();
                return;
            }

            double inerte;
            if (double.TryParse(li2.Text, out inerte))
            {
                if (inerte <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    li2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                li2.Focus();
                return;
            }

            double startp;
            if (double.TryParse(lix1.Text, out startp))
            {
                if (startp < 0 || startp >= beamlength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    lix1.Focus();
                    return;
                }
                if (inertiappoly.Cast<Poly>().Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    lix1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                lix1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(lix2.Text, out endp))
            {
                if (endp > beamlength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    lix2.Focus();
                    return;
                }
                if (inertiappoly.Cast<Poly>().Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    lix2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                lix2.Focus();
                return;
            }

            stresscbx.IsEnabled = false;
            var m = (Convert.ToDouble(li2.Text) - Convert.ToDouble(li1.Text)) /
            (Convert.ToDouble(lix2.Text) - Convert.ToDouble(lix1.Text));

            var c = -m * Convert.ToDouble(lix2.Text);
            var plytxt = "";
            if (c > 0)
            {
                plytxt = m + "x+" + c + "+" + Convert.ToDouble(li2.Text);
            }
            else
            {
                plytxt = m + "x" + c + "+" + Convert.ToDouble(li2.Text);
            }

            var poly = new Poly(plytxt);
            poly.StartPoint = Convert.ToDouble(lix1.Text);
            poly.EndPoint = Convert.ToDouble(lix2.Text);
            inertiappoly.Add(poly);

            if ((bool)stresscbx.IsChecked)
            {
                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }
                var epoly = new Poly(eli.Text);
                epoly.StartPoint = Convert.ToDouble(lix1.Text);
                epoly.EndPoint = Convert.ToDouble(lix2.Text);
                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }
                var dpoly = new Poly(dli.Text);
                dpoly.StartPoint = Convert.ToDouble(lix1.Text);
                dpoly.EndPoint = Convert.ToDouble(lix2.Text);
                dppoly.Add(dpoly);
            }

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void vibtn_Click(object sender, RoutedEventArgs e)
        {
            stresscbx.IsEnabled = false;

            double startp;
            if (double.TryParse(vix1.Text, out startp))
            {
                if (startp < 0 || startp >= beamlength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    vix1.Focus();
                    return;
                }
                if (inertiappoly.Cast<Poly>().Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    vix1.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                vix1.Focus();
                return;
            }

            double endp;
            if (double.TryParse(vix2.Text, out endp))
            {
                if (endp > beamlength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    vix2.Focus();
                    return;
                }
                if (inertiappoly.Cast<Poly>().Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    vix2.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                vix2.Focus();
                return;
            }

            if (!Poly.ValidateExpression(vi.Text))
            {
                MessageBox.Show(GetString("invalidpolynomial"));
                vi.Focus();
                return;
            }

            Poly poly;

            try
            {
                poly = new Poly(vi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(GetString("invalidpolynomial"));
                vi.Focus();
                return;
            }

            poly.StartPoint = Convert.ToDouble(vix1.Text);
            poly.EndPoint = Convert.ToDouble(vix2.Text);

            if (poly.Minimum(poly.StartPoint, poly.EndPoint) <= 0)
            {
                MessageBox.Show(GetString("minusinertia"));
                vi.Focus();
                return;
            }         

            inertiappoly.Add(poly);

            if ((bool)stresscbx.IsChecked)
            {
                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }
                var epoly = new Poly(evi.Text);
                epoly.StartPoint = Convert.ToDouble(vix1.Text);
                epoly.EndPoint = Convert.ToDouble(vix2.Text);
                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }
                var dpoly = new Poly(dvi.Text);
                dpoly.StartPoint = Convert.ToDouble(vix1.Text);
                dpoly.EndPoint = Convert.ToDouble(vix2.Text);
                dppoly.Add(dpoly);
            }

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);

            resetpanel();
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            if (validateinertia())
            {
                try
                {
                    if (length.Text != "")
                    {
                        beamlength = Convert.ToDouble(length.Text);
                    }
                    else
                    {
                        MessageBox.Show(GetString("invalidbeamlength"));
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(GetString("invalidbeamlength"));
                    return;
                }

                try
                {
                    if (elasticitymodulus.Text != "")
                    {
                        beamelasticitymodulus = Convert.ToDouble(elasticitymodulus.Text);
                        elasticitymodulus.Background = new SolidColorBrush(Color.FromArgb(200, 48, 247, 66));
                    }
                    else
                    {
                        MessageBox.Show(GetString("invalidelasticity"));
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(GetString("invalidelasticity"));
                    return;
                }

                try
                {
                    if (angletbx.Text != "")
                    {
                        angle = Convert.ToDouble(angletbx.Text);
                    }
                    else
                    {
                        MessageBox.Show(GetString("invalidangle"));
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(GetString("invalidangle"));
                    return;
                }

                DialogResult = true;
            }
        }

        private void uiexpand_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_loaded)
                {
                    ui.IsEnabled = true;
                    uix1.IsEnabled = true;
                    uix2.IsEnabled = true;
                    uibtn.IsEnabled = true;

                    if (inertiappoly.Length == 0)
                    {
                        ui.Text = "1";
                        uix1.Text = "0";
                        uix2.Text = beamlength.ToString();
                    }

                    li1.IsEnabled = false;
                    li1.Text = "";
                    li2.IsEnabled = false;
                    li2.Text = "";
                    lix1.IsEnabled = false;
                    lix1.Text = "";
                    lix2.IsEnabled = false;
                    lix2.Text = "";
                    libtn.IsEnabled = false;

                    vi.IsEnabled = false;
                    vi.Text = "";
                    vix1.IsEnabled = false;
                    vix1.Text = "";
                    vix2.IsEnabled = false;
                    vix2.Text = "";
                    vibtn.IsEnabled = false;

                    liexpand.IsExpanded = false;
                    viexpand.IsExpanded = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private void liexpand_Expanded(object sender, RoutedEventArgs e)
        {
            li1.IsEnabled = true;
            li2.IsEnabled = true;
            lix1.IsEnabled = true;
            lix2.IsEnabled = true;
            libtn.IsEnabled = true;

            if (inertiappoly.Length == 0)
            {
                li1.Text = "1";
                lix1.Text = "0";
                li2.Text = "1";
                lix2.Text = beamlength.ToString();
            }

            ui.IsEnabled = false;
            ui.Text = "";
            uix1.IsEnabled = false;
            uix1.Text = "";
            uix2.IsEnabled = false;
            uix2.Text = "";
            uibtn.IsEnabled = false;

            vi.IsEnabled = false;
            vi.Text = "";
            vix1.IsEnabled = false;
            vix1.Text = "";
            vix2.IsEnabled = false;
            vix2.Text = "";
            vibtn.IsEnabled = false;

            uiexpand.IsExpanded = false;
            viexpand.IsExpanded = false;
        }

        private void viexpand_Expanded(object sender, RoutedEventArgs e)
        {
            vi.IsEnabled = true;
            vix1.IsEnabled = true;
            vix2.IsEnabled = true;
            vibtn.IsEnabled = true;

            if (inertiappoly.Length == 0)
            {
                vi.Text = "1";
                vix1.Text = "0";
                vix2.Text = beamlength.ToString();
            }

            ui.IsEnabled = false;
            ui.Text = "";
            uix1.IsEnabled = false;
            uix1.Text = "";
            uix2.IsEnabled = false;
            uix2.Text = "";
            uibtn.IsEnabled = false;

            li1.IsEnabled = false;
            li1.Text = "";
            li2.IsEnabled = false;
            li2.Text = "";
            lix1.IsEnabled = false;
            lix1.Text = "";
            lix2.IsEnabled = false;
            lix2.Text = "";
            libtn.IsEnabled = false;

            uiexpand.IsExpanded = false;
            liexpand.IsExpanded = false;
        }

        private void resetpanel()
        {
            ui.IsEnabled = false;
            ui.Text = "";
            uix1.IsEnabled = false;
            uix1.Text = "";
            uix2.IsEnabled = false;
            uix2.Text = "";
            uibtn.IsEnabled = false;

            li1.IsEnabled = false;
            li1.Text = "";
            li2.IsEnabled = false;
            li2.Text = "";
            lix1.IsEnabled = false;
            lix1.Text = "";
            lix2.IsEnabled = false;
            lix2.Text = "";
            libtn.IsEnabled = false;

            vi.IsEnabled = false;
            vi.Text = "";
            vix1.IsEnabled = false;
            vix1.Text = "";
            vix2.IsEnabled = false;
            vix2.Text = "";
            vibtn.IsEnabled = false;

            uiexpand.IsExpanded = false;
            liexpand.IsExpanded = false;
            viexpand.IsExpanded = false;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var stk = (sender as Button).Parent as StackPanel;
            var fnc = stk.Parent as InertiaFunction;
            var index = fncstk.Children.IndexOf(fnc);
            inertiappoly.RemoveAt(index);
            if ((bool) stresscbx.IsChecked)
            {
                dppoly?.RemoveAt(index);
                eppoly?.RemoveAt(index);
            }
            fncstk.Children.RemoveAt(index);

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Collapsed;
            }
        }

        private void length_GotFocus(object sender, RoutedEventArgs e)
        {
            length.SelectionStart = 0;
            length.SelectionLength = length.Text.Length;
        }

        private void elasticitymodulus_GotFocus(object sender, RoutedEventArgs e)
        {
            elasticitymodulus.SelectionStart = 0;
            elasticitymodulus.SelectionLength = elasticitymodulus.Text.Length;
        }

        private void angletbx_GotFocus(object sender, RoutedEventArgs e)
        {
            angletbx.SelectionStart = 0;
            angletbx.SelectionLength = angletbx.Text.Length;
        }

        private double getAngle(Point start, Point end)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double angle = 0;

            angle = Math.Atan2(dy, dx) * 180 / Math.PI;

            return angle;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            maxstressstk.Visibility = Visibility.Visible;
            uistressanalyzestk.Visibility = Visibility.Visible;
            listressanalyzestk.Visibility = Visibility.Visible;
            vistressanalyzestk.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            maxstressstk.Visibility = Visibility.Collapsed;
            uistressanalyzestk.Visibility = Visibility.Collapsed;
            listressanalyzestk.Visibility = Visibility.Collapsed;
            vistressanalyzestk.Visibility = Visibility.Collapsed;
        }

        private bool validateinertia()
        {
            for (int i = 1; i < inertiappoly.Count; i++)
            {
                if (inertiappoly[i].StartPoint != inertiappoly[i-1].EndPoint)
                {
                    MessageBox.Show(GetString("notcoveredinertia"));                 
                    return false;
                }
            }

            if (inertiappoly[0].StartPoint != 0 || inertiappoly.Last().EndPoint != beamlength)
            {
                MessageBox.Show(GetString("notcoveredinertia"));
                return false;
            }

            return true;
        }

        private void updatefncstk()
        {
            foreach (Poly ipoly in _existingbeam.Inertias)
            {
                var fnc = new InertiaFunction();
                fnc.function.Text = "I(x) = " + ipoly.ToString();
                fnc.limits.Text = ipoly.StartPoint + " <= x <= " + ipoly.EndPoint;
                fnc.removebtn.Click += Remove_Click;
                fncstk.Children.Add(fnc);
            }
        }
    }
}
