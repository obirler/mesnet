using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.Tools;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LeftFixedSupport.xaml
    /// </summary>
    public partial class LeftFixedSupport : UserControl
    {
        public LeftFixedSupport(Canvas canvas)
        {
            InitializeComponent();
            _canbedragged = true;
            canvas.Children.Add(this);
            _id = AddObject(this);
            SupportCount++;
            _name = "Left Fixed Support " + SupportCount;
        }

        private int _id;

        private string _name;

        private bool _canbedragged;

        private bool _selected = false;

        public Member Member;

        private int _crossindex;

        public void Add(Canvas canvas)
        {
            canvas.Children.Add(this);
            _id = AddObject(this);
        }

        public void Add(Canvas canvas, double x, double y)
        {
            canvas.Children.Add(this);
            _id = AddObject(this);

            Canvas.SetLeft(this, x);

            Canvas.SetTop(this, y);
        }

        public void Add(Canvas canvas, Point point)
        {
            canvas.Children.Add(this);
            _id = AddObject(this);

            var x = point.X;
            var y = point.Y;

            Canvas.SetLeft(this, x - 7);

            Canvas.SetTop(this, y - 13);
        }

        public void AddBeam(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - 7);

            Canvas.SetTop(this, beam.LeftPoint.Y - 13);

            Member = new Member(beam, Direction.Left);

            beam.LeftSide = this;

            SetAngle(beam.Angle);
        }

        public void UpdatePosition(Beam beam)
        {
            Canvas.SetLeft(this, beam.LeftPoint.X - 7);

            Canvas.SetTop(this, beam.LeftPoint.Y - 13);

            SetAngle(beam.Angle);
        }

        public void Select()
        {
            p1.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p2.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p3.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p4.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p5.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            p6.Fill = new SolidColorBrush(Color.FromArgb(180, 255, 165, 0));
            _selected = true;
        }

        public void UnSelect()
        {
            p1.Fill = new SolidColorBrush(Colors.Black);
            p2.Fill = new SolidColorBrush(Colors.Black);
            p3.Fill = new SolidColorBrush(Colors.Black);
            p4.Fill = new SolidColorBrush(Colors.Black);
            p5.Fill = new SolidColorBrush(Colors.Black);
            p6.Fill = new SolidColorBrush(Colors.Black);
            _selected = false;
        }

        private bool IsSelected()
        {
            return _selected;
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetPosition(double x, double y)
        {
            if (_canbedragged)
            {
                var left = x - Width / 2;
                var right = y - Height / 2;

                Canvas.SetLeft(this, left);

                Canvas.SetTop(this, right);

                MyDebug.WriteInformation("LeftFixedSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("LeftFixedSupport", "The beam to be dragged can not be dragged");
            }
        }

        /// <summary>
        /// Sets the position in the canvas based on the center point of the element.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetPosition(Point point)
        {
            if (_canbedragged)
            {
                var left = point.X - Width / 2;
                var right = point.Y - Height / 2;

                Canvas.SetLeft(this, left);

                Canvas.SetTop(this, right);

                MyDebug.WriteWarning("LeftFixedSupport", "Position has been set : " + left + " : " + right);
            }
            else
            {
                MyDebug.WriteWarning("LeftFixedSupport", "The beam to be dragged can not be dragged");
            }
        }

        public void SetAngle(double angle)
        {
            rotateTransform.Angle = angle;
        }

        #region Properties

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the beam can be dragged in the canvas.
        /// </summary>
        /// <value>
        /// <c>true</c> if the beam can be dragged; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeDragged
        {
            get
            {
                return _canbedragged;
            }
            set
            {
                _canbedragged = value;
            }
        }

        public double MomentDifference
        {
            get { return Member.Moment; }
        }

        public int CrossIndex
        {
            get { return _crossindex; }
            set { _crossindex = value; }
        }
        #endregion
    }
}
