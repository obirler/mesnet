using System.Text;

namespace Mesnet.Classes.IO.Reporter
{
    /// <summary>
    /// Base class for reports
    /// </summary>
    public abstract class Report
    {
        protected StringBuilder _sb;

        protected Global.ReportType _type;

        public string GetContent
        {
            get
            {
                return _sb.ToString();
            }
        }
    }
}
