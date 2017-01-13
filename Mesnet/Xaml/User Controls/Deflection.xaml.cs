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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes;
using Mesnet.Classes.Ui;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Deflection.xaml
    /// </summary>
    public partial class Deflection : UserControl
    {
        public Deflection(List<Global.Func> deflectionlist, Beam beam)
        {
            InitializeComponent();

            _beam = beam;
            _deflection = deflectionlist;
            _max = beam.MaxDeflection;

            coeff = 200 / Global.MaxDeflection;

            draw();
        }

        private List<Global.Func> _deflection;

        private Beam _beam;

        private Global.Func _max;

        private MainWindow _mw = (MainWindow)Application.Current.MainWindow;

        private double coeff;

        private void draw()
        {
            var points = new PointCollection();
            for (int i = 0; i < _deflection.Count; i++)
            {
                var point = new Point(_deflection[i].xposition * 100, _deflection[i].yposition * coeff);
                points.Add(point);
            }

            var spline = new CardinalSplineShape(points);
            spline.Stroke = new SolidColorBrush(Colors.OrangeRed);
            spline.StrokeThickness = 1;
            //spline.MouseMove += _mw.mousemove;
            //spline.MouseEnter += _mw.mouseenter;
            //spline.MouseLeave += _mw.mouseleave;
            deflectioncanvas.Children.Add(spline);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
