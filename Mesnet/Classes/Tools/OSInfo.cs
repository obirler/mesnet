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

using System.Management;
using System.Text;
using Microsoft.Win32;

namespace Mesnet.Classes.Tools
{
    public static class OSInfo
    {
        /// <summary>
        /// Return total physical memory in MB.
        /// </summary>
        /// <returns>Total physical memory</returns>
        public static string TotalAvailableRAM()
        {
            int ram = (int) ((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory) / (1000 * 1024));
            return ram + " MB";
        }

        public static string OperatingSystemName()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName;
        }

        public static string OperatingSystemVersion()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSVersion;
        }

        /// <summary>
        /// Returns installed .Net versions.
        /// </summary>
        /// <returns></returns>
        public static string DotNetVersions()
        {
            StringBuilder info = new StringBuilder();
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5 
                // or later, you can use:
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("MesnetVersion", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                        {
                            info.Append((versionKeyName + "  " + name + "/r/n"));
                        }
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                info.Append(versionKeyName + "  " + name + "  SP" + sp + "/r/n");
                            }
                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("MesnetVersion", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                            {
                                info.Append(versionKeyName + "  " + name + "/r/n");
                            }
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    info.Append("  " + subKeyName + "  " + name + "  SP" + sp + "/r/n");
                                }
                                else if (install == "1")
                                {
                                    info.Append("  " + subKeyName + "  " + name + "/r/n");
                                }
                            }
                        }
                    }
                }
            }
            return info.ToString();
        }

        public static string ProcessorName()
        {
            var info = new StringBuilder();
            using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor"))
            {
                foreach (ManagementObject obj in win32Proc.Get())
                {
                    info.Append(obj["Name"] + " ");
                }
            }
            return info.ToString();
        }

        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
        public static string GetVolumeSerial(string strDriveLetter= "C")
        {
            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";
            ManagementObject disk =
                new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }
    }
}
