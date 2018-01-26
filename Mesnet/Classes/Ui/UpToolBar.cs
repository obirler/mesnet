using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Classes.Ui
{
    public class UpToolBar
    {
        public UpToolBar(MainWindow mw)
        {
            _mw = mw;

            bindevents();

            hideall();
        }

        private MainWindow _mw;

        private bool _inertiaslider = true;

        private bool _distloadslider = true;

        private bool _concloadslider = true;

        private bool _momentslider = true;

        private bool _forceslider = true;

        private bool _stressslider = true;

        private void hideall()
        {
            _mw.inertiaborder.Visibility = Visibility.Collapsed;
            _mw.distloadborder.Visibility = Visibility.Collapsed;
            _mw.concloadborder.Visibility = Visibility.Collapsed;
            _mw.momentborder.Visibility = Visibility.Collapsed;
            _mw.forceborder.Visibility = Visibility.Collapsed;
            _mw.stressborder.Visibility = Visibility.Collapsed;
        }

        private void bindevents()
        {
            _mw.solvebtn.Click += solvebtn_Click;

            _mw.inertiaexpander.Expanded += inertiaexpander_Expanded;
            _mw.inertiaexpander.Collapsed += inertiaexpander_Collapsed;
            _mw.inertiaslider.ValueChanged += inertiaslider_ValueChanged;

            _mw.distloadexpander.Expanded += distloadexpander_Expanded;
            _mw.distloadexpander.Collapsed += distloadexpander_Collapsed;
            _mw.distloadslider.ValueChanged += distloadslider_ValueChanged;

            _mw.concloadexpander.Expanded += concloadexpander_Expanded;
            _mw.concloadexpander.Collapsed += concloadexpander_Collapsed;
            _mw.concloadslider.ValueChanged += concloadslider_ValueChanged;

            _mw.momentexpander.Expanded += momentexpander_Expanded;
            _mw.momentexpander.Collapsed += momentexpander_Collapsed;
            _mw.momentslider.ValueChanged += momentslider_ValueChanged;

            _mw.forceexpander.Expanded += forceexpander_Expanded;
            _mw.forceexpander.Collapsed += forceexpander_Collapsed;
            _mw.forceslider.ValueChanged += forceslider_ValueChanged;

            _mw.stressexpander.Expanded += stressexpander_Expanded;
            _mw.stressexpander.Collapsed += stressexpander_Collapsed;
            _mw.stressslider.ValueChanged += stressslider_ValueChanged;
        }

        private void solvebtn_Click(object sender, RoutedEventArgs e)
        {
            _mw.InitializeSolution();
        }

        public void ActivateInertia()
        {
            _mw.inertiaborder.Visibility = Visibility.Visible;
        }

        public void DeActivateInertia()
        {
            _mw.inertiaborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateDistLoad()
        {
            _mw.distloadborder.Visibility = Visibility.Visible;
        }

        public void DeActivateDistLoad()
        {
            _mw.distloadborder.Visibility = Visibility.Collapsed;
        }

        public void ShowDistLoad()
        {
            _mw.distloadexpander.IsExpanded = true;
        }

        public void ActivateConcLoad()
        {
            _mw.concloadborder.Visibility = Visibility.Visible;
        }

        public void DeActivateConcLoad()
        {
            _mw.concloadborder.Visibility = Visibility.Collapsed;
        }

        public void ShowConcLoad()
        {
            _mw.concloadexpander.IsExpanded = true;
        }

        public void ActivateMoment()
        {
            _mw.momentborder.Visibility = Visibility.Visible;
        }

        public void DeActivateMoment()
        {
            _mw.momentborder.Visibility = Visibility.Collapsed;
        }

        public void ShowMoments()
        {
            _mw.momentexpander.IsExpanded = true;
        }

        public void ActivateForce()
        {
            _mw.forceborder.Visibility = Visibility.Visible;
        }

        public void DeActivateForce()
        {
            _mw.forceborder.Visibility = Visibility.Collapsed;
        }

        public void ActivateStress()
        {
            _mw.stressborder.Visibility = Visibility.Visible;
        }

        public void DeActivateStress()
        {
            _mw.stressborder.Visibility = Visibility.Collapsed;
        }

        public void CollapseInertia()
        {
            _mw.inertiaexpander.IsExpanded = false;
        }

        public void CollapseDistLoad()
        {
            _mw.distloadexpander.IsExpanded = false;
        }

        public void CollapseConcLoad()
        {
            _mw.concloadexpander.IsExpanded = false;
        }

        public void CollapseMoment()
        {
            _mw.momentexpander.IsExpanded = false;
        }

        public void CollapseForce()
        {
            _mw.forceexpander.IsExpanded = false;
        }

        public void CollapseStress()
        {
            _mw.stressexpander.IsExpanded = false;
        }

        #region Toolbar Events

        private void inertiaexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.inertiaborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowInertiaDiagram((int)_mw.inertiaslider.Value);

                        break;
                }
            }
        }

        private void inertiaexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.inertiaborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideInertiaDiagram();

                        break;
                }
            }
        }

        private void inertiaslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("inertiaslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        beam.ReDrawInertia((int)e.NewValue);

                        break;
                }
            }
        }

        private void distloadexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.distloadborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowDistLoadDiagram((int)_mw.distloadslider.Value);

                        break;
                }
            }
        }

        private void distloadexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.distloadborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideDistLoadDiagram();

                        break;
                }
            }
        }

        private void distloadslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("distloadslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        if (beam.DistributedLoads != null)
                        {
                            if (beam.DistributedLoads.Count > 0)
                            {
                                beam.ReDrawDistLoad((int)e.NewValue);
                            }
                        }
                        break;
                }
            }
        }

        private void concloadexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.concloadborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowConcLoadDiagram((int)_mw.concloadslider.Value);

                        break;
                }
            }
        }

        private void concloadexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.concloadborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideConcLoadDiagram();

                        break;
                }
            }
        }

        private void concloadslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("distloadslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        if (beam.ConcentratedLoads != null)
                        {
                            if (beam.ConcentratedLoads.Count > 0)
                            {
                                beam.ReDrawConcLoad((int)e.NewValue);
                            }
                        }
                        break;
                }
            }
        }

        private void momentexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.momentborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowFixedEndMomentDiagram((int)_mw.momentslider.Value);

                        break;
                }
            }
        }

        private void momentexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.momentborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideFixedEndMomentDiagram();

                        break;
                }
            }
        }

        private void momentslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("momentslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        beam.ReDrawMoment((int)e.NewValue);

                        break;
                }
            }
        }

        private void forceexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.forceborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowFixedEndForceDiagram((int)_mw.forceslider.Value);

                        break;
                }
            }
        }

        private void forceexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.forceborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideFixedEndForceDiagram();

                        break;
                }
            }
        }

        private void forceslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("forceslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        beam.ReDrawForce((int)e.NewValue);

                        break;
                }
            }
        }

        private void stressexpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mw.stressborder.Background = new SolidColorBrush(Color.FromArgb(255, 188, 221, 255));

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.ShowStressDiagram((int)_mw.stressslider.Value);

                        break;
                }
            }
        }

        private void stressexpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mw.stressborder.Background = new SolidColorBrush(Colors.WhiteSmoke);

            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case ObjectType.Beam:

                        Beam beam = item as Beam;
                        beam.HideStressDiagram();

                        break;
                }
            }
        }

        private void stressslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyDebug.WriteInformation("stressslider value changed : " + e.NewValue);
            foreach (var item in Objects)
            {
                switch (GetObjectType(item))
                {
                    case Global.ObjectType.Beam:

                        var beam = item as Beam;
                        beam.ReDrawStress((int)e.NewValue);

                        break;
                }
            }
        }
        
        #endregion
    }
}
