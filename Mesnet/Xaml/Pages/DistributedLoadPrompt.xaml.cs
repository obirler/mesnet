using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mesnet.Classes.Math;
using Mesnet.Xaml.User_Controls;

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
