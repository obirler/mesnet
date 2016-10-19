using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Classes.Tools
{
    public class Member
    {
        public Member()
        {
            
        }

        public Member(Beam beam, Direction direction)
        {
            _beam = beam;
            _direction = direction;
        }

        private Beam _beam;

        private Direction _direction;

        public Beam Beam
        {
            get { return _beam; }
            set { _beam = value; }
        }

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public double Moment
        {
            get
            {
                double moment = 0;
                switch (_direction)
                {
                    case Direction.Left:

                        moment = _beam.LeftEndMoment;

                        break;

                    case Direction.Right:

                        moment = _beam.RightEndMoment;

                        break;
                }

                return moment;
            }
        }
    }
}
