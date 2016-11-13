using System;
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
            beamlength = Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2)) / 100;
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

        public double beamlength = 0;

        public double beamelasticitymodulus = 0;

        public double beaminertiamodulus = 0;

        public double angle = 0;

        public PiecewisePoly inertiappoly;

        public PiecewisePoly eppoly;

        public PiecewisePoly dppoly;

        public FunctionType type;

        private bool _loaded = false;

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
            var poly = new Poly(vi.Text);
            poly.StartPoint = Convert.ToDouble(vix1.Text);
            poly.EndPoint = Convert.ToDouble(vix2.Text);

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
            beamlength = Convert.ToDouble(length.Text);
            beamelasticitymodulus = Convert.ToDouble(elasticitymodulus.Text);
            angle = Convert.ToDouble(angletbx.Text);
            DialogResult = true;
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
    }
}
