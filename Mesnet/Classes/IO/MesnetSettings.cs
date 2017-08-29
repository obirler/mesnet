using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Mesnet.Classes.IO
{
    public static class MesnetSettings
    {
        //Static constructor that runs only once when one of the static methods of the class called at the first time
        static MesnetSettings()
        {
            if (!File.Exists(filename))
            {
                createfile();
            }
            _doc = XDocument.Load(filename);
        }

        private const string filename = "settings.cfg";

        static XDocument _doc;

        public static void WriteSetting(string settingname, string value, string group)
        {           
            if(!IsSettingGroupExists(group))
            {
                createsettinggroup(group);
            }

            if(IsSettingExists(settingname, group))
            {               
                foreach (XElement groupelement in _doc.Element("MesnetSettings").Elements())
                {
                    if (groupelement.Name == group)
                    {
                        foreach(XElement settingelement in groupelement.Elements())
                        {
                            if(settingelement.Name == settingname)
                            {
                                settingelement.Value = value;
                                _doc.Save(filename);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (XElement groupelement in _doc.Element("MesnetSettings").Elements())
                {
                    if (groupelement.Name == group)
                    {
                        var settingelement = new XElement(settingname, value);
                        groupelement.Add(settingelement);
                        _doc.Save(filename);
                        return;
                    }
                }
            }
        }

        public static string ReadSetting(string settingname, string group)
        {
            foreach (XElement groupelement in _doc.Element("MesnetSettings").Elements())
            {
                if (groupelement.Name == group)
                {
                    foreach (XElement settingelement in groupelement.Elements())
                    {
                        if (settingelement.Name == settingname)
                        {
                            return settingelement.Value;
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsSettingExists(string settingname, string group)
        {
            foreach (XElement groupelement in _doc.Element("MesnetSettings").Elements())
            {
                if (groupelement.Name == group)
                {
                    foreach (XElement settingelement in groupelement.Elements())
                    {
                        if(settingelement.Name == settingname)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        public static bool IsSettingGroupExists(string group)
        {
            foreach (XElement groupelement in _doc.Element("MesnetSettings").Elements())
            {
                if (groupelement.Name == group)
                {                    
                    return true;
                }
            }
            return false;
        }

        private static void createsettinggroup(string group)
        {
            _doc.Root.Add(new XElement(group));
            _doc.Save(filename);
        }

        public static void createfile()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("MesnetSettings");              
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
