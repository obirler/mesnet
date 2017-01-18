using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mesnet.Classes;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for SupportBeamItem.xaml
    /// </summary>
    public partial class SupportBeamItem : UserControl
    {
        public SupportBeamItem(int beamid, Direction direction, double moment)
        {
            InitializeComponent();
            switch (direction)
            {
                case Direction.Right:
                    header.Text = GetString("beam") + " " + beamid + " , " + GetString("rightside") + ",  " +
                                  Math.Round(moment, 4) + " kNm";
                    break;

                case Direction.Left:
                    header.Text = GetString("beam") + " " + beamid + " , " + GetString("leftside") + ",  " +
                                  Math.Round(moment, 4) + " kNm";
                    break;
            }

        }
    }
}
