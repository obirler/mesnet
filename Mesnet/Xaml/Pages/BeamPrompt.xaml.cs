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

            _loaded = true;

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
            if (!checkui())
            {
                return;
            }

            if (!checkstartendpoint(uix1, uix2))
            {
                return;
            }

            double startpoint = Convert.ToDouble(uix1.Text);

            double endpoint = Convert.ToDouble(uix2.Text);

            if ((bool)stresscbx.IsChecked)
            {
                if (!checkuie())
                {
                    return;
                }

                double evalue = Convert.ToDouble(eui.Text);

                if (!checkuid(evalue))
                {
                    return;
                }

                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }

                var epoly = new Poly(eui.Text);
                epoly.StartPoint = startpoint;
                epoly.EndPoint = endpoint;
                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }

                var dpoly = new Poly(dui.Text);
                dpoly.StartPoint = startpoint;
                dpoly.EndPoint = endpoint;
                dppoly.Add(dpoly);
            }

            var ineritiapoly = new Poly(ui.Text);
            ineritiapoly.StartPoint = startpoint;
            ineritiapoly.EndPoint = endpoint;

            inertiappoly.Add(ineritiapoly);

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + ineritiapoly.ToString();
            fnc.limits.Text = ineritiapoly.StartPoint + " <= x <= " + ineritiapoly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);
            stresscbx.IsEnabled = false;
            resetpanel();
        }

        private void libtn_Click(object sender, RoutedEventArgs e)
        {          
            if (!checkli())
            {
                return;
            }

            if (!checkstartendpoint(lix1, lix2))
            {
                return;
            }

            double startpoint = Convert.ToDouble(lix1.Text);

            double endpoint = Convert.ToDouble(lix2.Text);         

            if ((bool)stresscbx.IsChecked)
            {
                if (!checkepoly(eli, startpoint, endpoint))
                {
                    return;
                }

                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }
                var epoly = new Poly(eli.Text);
                epoly.StartPoint = startpoint;
                epoly.EndPoint = endpoint;

                if (!checkdpoly(dli, startpoint, endpoint, epoly))
                {
                    return;
                }
               
                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }
                var dpoly = new Poly(dli.Text);
                dpoly.StartPoint = startpoint;
                dpoly.EndPoint = endpoint;
                dppoly.Add(dpoly);
            }

            double inertiastart = Convert.ToDouble(li1.Text);
            double inertiaend = Convert.ToDouble(li2.Text);

            var m = (inertiaend - inertiastart) / (endpoint - startpoint);
            var c = -m * endpoint;

            var polytxt = "";
            if (c > 0)
            {
                polytxt = m + "x+" + c + "+" + inertiaend;
            }
            else
            {
                polytxt = m + "x" + c + "+" + inertiaend;
            }

            var poly = new Poly(polytxt);
            poly.StartPoint = startpoint;
            poly.EndPoint = endpoint;
            inertiappoly.Add(poly);

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);
            stresscbx.IsEnabled = false;
            resetpanel();
        }

        private void vibtn_Click(object sender, RoutedEventArgs e)
        {
            if (!checkstartendpoint(vix1, vix2))
            {
                return;
            }

            double startpoint = Convert.ToDouble(vix1.Text);

            double endpoint = Convert.ToDouble(vix2.Text);

            if (!checkvi(startpoint, endpoint))
            {
                return;
            }

            if ((bool)stresscbx.IsChecked)
            {
                if (!checkepoly(evi, startpoint, endpoint))
                {
                    return;
                }

                if (eppoly == null)
                {
                    eppoly = new PiecewisePoly();
                }
                var epoly = new Poly(evi.Text);
                epoly.StartPoint = startpoint;
                epoly.EndPoint = endpoint;

                if (!checkdpoly(dvi, startpoint, endpoint, epoly))
                {
                    return;
                }

                eppoly.Add(epoly);

                if (dppoly == null)
                {
                    dppoly = new PiecewisePoly();
                }
                var dpoly = new Poly(dvi.Text);
                dpoly.StartPoint = startpoint;
                dpoly.EndPoint = endpoint;
                dppoly.Add(dpoly);
            }

            Poly poly = new Poly(vi.Text);
            poly.StartPoint = startpoint;
            poly.EndPoint = endpoint;
            inertiappoly.Add(poly);

            var fnc = new InertiaFunction();
            fnc.function.Text = "I(x) = " + poly.ToString();
            fnc.limits.Text = poly.StartPoint + " <= x <= " + poly.EndPoint;
            fnc.removebtn.Click += Remove_Click;

            if (fncstk.Children.Count == 0)
            {
                finishbtn.Visibility = Visibility.Visible;
            }
            fncstk.Children.Add(fnc);
            stresscbx.IsEnabled = false;
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
                    eui.IsEnabled = true;
                    dui.IsEnabled = true;

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
                    eli.IsEnabled = false;
                    eli.Text = "";
                    dli.IsEnabled = false;
                    dli.Text = "";

                    vi.IsEnabled = false;
                    vi.Text = "";
                    vix1.IsEnabled = false;
                    vix1.Text = "";
                    vix2.IsEnabled = false;
                    vix2.Text = "";
                    vibtn.IsEnabled = false;
                    evi.IsEnabled = false;
                    evi.Text = "";
                    dvi.IsEnabled = false;
                    dvi.Text = "";

                    liexpand.IsExpanded = false;
                    viexpand.IsExpanded = false;
                }
            }
            catch (Exception)
            {}
        }

        private void liexpand_Expanded(object sender, RoutedEventArgs e)
        {
            li1.IsEnabled = true;
            li2.IsEnabled = true;
            lix1.IsEnabled = true;
            lix2.IsEnabled = true;
            libtn.IsEnabled = true;
            eli.IsEnabled = true;
            dli.IsEnabled = true;

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
            eui.IsEnabled = false;
            eui.Text = "";
            dui.IsEnabled = false;
            dui.Text = "";

            vi.IsEnabled = false;
            vi.Text = "";
            vix1.IsEnabled = false;
            vix1.Text = "";
            vix2.IsEnabled = false;
            vix2.Text = "";
            vibtn.IsEnabled = false;
            evi.IsEnabled = false;
            evi.Text = "";
            dvi.IsEnabled = false;
            dvi.Text = "";

            uiexpand.IsExpanded = false;
            viexpand.IsExpanded = false;
        }

        private void viexpand_Expanded(object sender, RoutedEventArgs e)
        {
            vi.IsEnabled = true;
            vix1.IsEnabled = true;
            vix2.IsEnabled = true;
            vibtn.IsEnabled = true;
            evi.IsEnabled = true;
            dvi.IsEnabled = true;

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
            eui.IsEnabled = false;
            eui.Text = "";
            dui.IsEnabled = false;
            dui.Text = "";

            li1.IsEnabled = false;
            li1.Text = "";
            li2.IsEnabled = false;
            li2.Text = "";
            lix1.IsEnabled = false;
            lix1.Text = "";
            lix2.IsEnabled = false;
            lix2.Text = "";
            libtn.IsEnabled = false;
            eli.IsEnabled = false;
            eli.Text = "";
            dli.IsEnabled = false;
            dli.Text = "";

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
            eui.IsEnabled = false;
            eui.Text = "";
            dui.IsEnabled = false;
            dui.Text = "";
            uibtn.IsEnabled = false;


            li1.IsEnabled = false;
            li1.Text = "";
            li2.IsEnabled = false;
            li2.Text = "";
            lix1.IsEnabled = false;
            lix1.Text = "";
            lix2.IsEnabled = false;
            lix2.Text = "";
            eli.IsEnabled = false;
            eli.Text = "";
            dli.IsEnabled = false;
            dli.Text = "";
            libtn.IsEnabled = false;

            vi.IsEnabled = false;
            vi.Text = "";
            vix1.IsEnabled = false;
            vix1.Text = "";
            vix2.IsEnabled = false;
            vix2.Text = "";
            evi.IsEnabled = false;
            evi.Text = "";
            dvi.IsEnabled = false;
            dvi.Text = "";
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
                stresscbx.IsEnabled = true;
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

        #region Checking Methods

        private bool checkstartendpoint(TextBox starttbx, TextBox endtbx)
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
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(GetString("invalidbeamlength"));
                return false;
            }

            double startp;
            if (double.TryParse(starttbx.Text, out startp))
            {
                if (startp < 0 || startp >= beamlength)
                {
                    MessageBox.Show(GetString("invalidstartpoint"));
                    starttbx.Focus();
                    return false;
                }
                if (inertiappoly.Cast<Poly>().Any(item => startp >= item.StartPoint && startp < item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    starttbx.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidstartpoint"));
                starttbx.Focus();
                return false;
            }

            double endp;
            if (double.TryParse(endtbx.Text, out endp))
            {
                if (endp > beamlength || endp <= startp)
                {
                    MessageBox.Show(GetString("invalidendpoint"));
                    endtbx.Focus();
                    return false;
                }

                if (inertiappoly.Cast<Poly>().Any(item => endp > item.StartPoint && endp <= item.EndPoint))
                {
                    MessageBox.Show(GetString("invalidrange"));
                    endtbx.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidendpoint"));
                endtbx.Focus();
                return false;
            }
            return true;
        }

        private bool checkui()
        {
            double inert;
            if (double.TryParse(ui.Text, out inert))
            {
                if (inert <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    ui.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                ui.Focus();
                return false;
            }
            return true;
        }

        private bool checkli()
        {
            double inertstart;
            if (double.TryParse(li1.Text, out inertstart))
            {
                if (inertstart <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    li1.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                li1.Focus();
                return false;
            }

            double inertend;
            if (double.TryParse(li2.Text, out inertend))
            {
                if (inertend <= 0)
                {
                    MessageBox.Show(GetString("minusinertia"));
                    li2.Focus();
                    return false;
                }
            }
            else
            {
                MessageBox.Show(GetString("invalidinertia"));
                li2.Focus();
                return false;
            }

            return true;
        }

        private bool checkvi(double startpoint, double endpoint)
        {
            if (!Poly.ValidateExpression(vi.Text))
            {
                MessageBox.Show(GetString("invalidpolynomial"));
                vi.Focus();
                return false;
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
                return false;
            }

            poly.StartPoint = startpoint;
            poly.EndPoint = endpoint;

            if (poly.Minimum(poly.StartPoint, poly.EndPoint) <= 0)
            {
                MessageBox.Show(GetString("minusinertia"));
                vi.Focus();
                return false;
            }

            return true;
        }

        private bool checkuie()
        {
            double evalue;

            if (!double.TryParse(eui.Text, out evalue))
            {
                MessageBox.Show(GetString("invalidevalue"));
                eui.Focus();
                return false;
            }

            if (evalue <= 0)
            {
                MessageBox.Show(GetString("invalidevalue"));
                eui.Focus();
                return false;
            }

            return true;
        }    

        private bool checkuid(double evalue)
        {
            double dvalue;
            if (!double.TryParse(dui.Text, out dvalue))
            {
                MessageBox.Show(GetString("invaliddvalue"));
                dui.Focus();
                return false;
            }

            if (dvalue <= 0)
            {
                MessageBox.Show(GetString("invaliddvalue"));
                dui.Focus();
                return false;
            }

            if (evalue >= dvalue)
            {
                MessageBox.Show(GetString("ehigheroregualtod"));
                dui.Focus();
                return false;
            }

            return true;
        }

        private bool checkepoly(TextBox activetbx, double startpoint, double endpoint)
        {
            if (!Poly.ValidateExpression(activetbx.Text))
            {
                MessageBox.Show(GetString("invalidevalue"));
                activetbx.Focus();
                return false;
            }

            Poly epoly;

            try
            {
                epoly = new Poly(activetbx.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(GetString("invalidevalue"));
                activetbx.Focus();
                return false;
            }

            epoly.StartPoint = startpoint;
            epoly.EndPoint = endpoint;

            if (epoly.Minimum(epoly.StartPoint, epoly.EndPoint) <= 0)
            {
                MessageBox.Show(GetString("invalidevalue"));
                activetbx.Focus();
                return false;
            }
            return true;
        }

        private bool checkdpoly(TextBox activetbx, double startpoint, double endpoint, Poly epoly)
        {
            if (!Poly.ValidateExpression(activetbx.Text))
            {
                MessageBox.Show(GetString("invaliddvalue"));
                activetbx.Focus();
                return false;
            }

            Poly dpoly;

            try
            {
                dpoly = new Poly(activetbx.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(GetString("invaliddvalue"));
                activetbx.Focus();
                return false;
            }

            dpoly.StartPoint = startpoint;
            dpoly.EndPoint = endpoint;

            if (dpoly.Minimum(dpoly.StartPoint, dpoly.EndPoint) <= 0)
            {
                MessageBox.Show(GetString("invaliddvalue"));
                activetbx.Focus();
                return false;
            }

            for (double i = startpoint; i <= endpoint; i=i+0.001)
            {
                if (epoly.Calculate(i) >= dpoly.Calculate(i))
                {
                    MessageBox.Show(GetString("ehigheroregualtod"));
                    activetbx.Focus();
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
