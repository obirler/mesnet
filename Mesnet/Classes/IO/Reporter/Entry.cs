using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesnet.Classes.IO.Reporter
{
    public class Entry
    {
        public Entry(string type, Report report)
        {
            _type = type;
            _report = report;
        }

        private string _type;

        private Report _report;

        public Log Create()
        {
            var log = new Log();
            log.AppName = Global.AppName;
            log.UserName = Global.UserName;
            string correctedcontent = _report.GetContent.Replace("'", "''");
            log.Content = correctedcontent;
            log.Type = _type;
            DateTime time = DateTime.Now.ToUniversalTime();
            string createdate = time.ToString("dd/MM/yyyy HH:mm:ss");
            log.CreatedDate = createdate;
            log.Version = Global.VersionNumber;
            return log;
        }
    }
}
