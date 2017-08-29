using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Mesnet.Classes.IO.Xml;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var setting = new Mesnet.Classes.IO.MesnetSettings();

            /*setting.WriteSetting("somename", "somevalue", "somegroup");

            setting.WriteSetting("someothername", "someothervalue", "somegroup");

            setting.WriteSetting("blabla", "notmmmm", "someothergroup");

            Console.WriteLine(setting.IsSettingExists("someothername", "somegroup"));

            Console.WriteLine(setting.ReadSetting("someothername", "somegroup"));

            Console.WriteLine(setting.IsSettingGroupExists("somegroup"));*/

            /*Console.WriteLine(setting.IsSettingExists("sslsfk", "wffşil"));

            Console.WriteLine(setting.IsSettingExists("number_3","squarenumbergroup"));

            Console.WriteLine(setting.IsSettingExists("number_300", "squarenumbergroup"));*/

            Console.WriteLine(setting.ReadSetting("number_3", "squarenumbergroup"));

            Console.WriteLine(setting.ReadSetting("number_300", "squarenumbergroup"));

            Console.WriteLine(setting.ReadSetting("number_3", "jkg"));

            Console.WriteLine("Finished");

            Console.ReadKey();
        }
    }
}
