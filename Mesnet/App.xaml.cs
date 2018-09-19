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

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesnet.Classes.IO.Reporter;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using System.Threading;
using Mesnet.Classes;
using Mesnet.Classes.IO;
using Mesnet.Classes.Reporter;

namespace Mesnet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Reporter.Close();
            Logger.CloseLogger();
            MesnetSettings.Close();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new
                UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            string[] arguments = System.Environment.GetCommandLineArgs();

            if (arguments.GetLength(0) > 1)
            {
                if (arguments[1].EndsWith(".mnt"))
                {
                    AssociationPath = arguments[1];
                }
            }
            Logger.InitializeLogger();

            readsettings();

            Reporter.TryToSend();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MesnetDebug.WriteError("Unhandled exception catched!!!");
            var crashcontent = new CrashReport(e.ExceptionObject, e.IsTerminating);
            var entry = new Entry("CrashReport", crashcontent);
            Reporter.Write(entry);
        }

        private void readsettings()
        {
            if (MesnetSettings.IsSettingExists("userid"))
            {
                if (MesnetSettings.IsSettingExists("userregistered"))
                {
                    switch (MesnetSettings.ReadSetting("userregistered"))
                    {
                        case "true":

                            //Do nothing. The user already registered.
                            MesnetDebug.WriteInformation("User is registered");

                            break;

                        case "false":

                            MesnetDebug.WriteInformation("User is not registered");
                            Reporter.RegisterUser();

                            break;
                    }
                }
                else
                {
                    MesnetSettings.WriteSetting("userregistered", "false");
                    Reporter.RegisterUser();
                }
            }
            else
            {
                string id = ThumbPrint.Value();
                MesnetSettings.WriteSetting("userid", id);
                MesnetDebug.WriteInformation("User id has been created");
                Reporter.RegisterUser();
            }

            if (MesnetSettings.IsSettingExists("launchnumber"))
            {
                string numberstr = MesnetSettings.ReadSetting("launchnumber");
                int number = Convert.ToInt32(numberstr);
                int newnumber = number + 1;
                MesnetSettings.WriteSetting("launchnumber", newnumber.ToString());
                if (newnumber == 10)
                {
                    MesnetDebug.WriteInformation("This is the 10th launch, Welcome!");
                    var report = new StringReport("User launched the application for 10th time");
                    var entry = new Entry("10th Launch", report);
                    Reporter.Write(entry);
                }
                else if (newnumber == 20)
                {
                    MesnetDebug.WriteInformation("This is the 20th launch, Welcome!");
                    var report = new StringReport("User launched the application for 20th time");
                    var entry = new Entry("20th Launch", report);
                    Reporter.Write(entry);
                }
                else if (newnumber == 50)
                {
                    MesnetDebug.WriteInformation("This is the 50th launch, Welcome!");
                    var report = new StringReport("User launched the application for 50th time");
                    var entry = new Entry("50th Launch", report);
                    Reporter.Write(entry);
                }
            }
            else
            {
                //First Launch
                MesnetSettings.WriteSetting("launchnumber", "1");
                MesnetDebug.WriteInformation("This is the first launch, Welcome!");

                var stringreport = new StringReport("User launched the application for the first time");
                var stringentry = new Entry("First Launch", stringreport);
                Reporter.Write(stringentry);

                var osreport = new OperatingSystemReport();
                var osentry = new Entry("Operating System Report", osreport);
                Reporter.Write(osentry);

            }

            if (App.Current.Resources.MergedDictionaries.Count != 0)
            {
                App.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            ResourceDictionary dict = new ResourceDictionary();

            if (MesnetSettings.IsSettingExists("language"))
            {
                switch (MesnetSettings.ReadSetting("language"))
                {
                    case "tr-TR":

                        Global.Language = Global.LanguageType.Turkish;
                        dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);

                        break;

                    case "en-EN":

                        Global.Language = Global.LanguageType.English;
                        dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);

                        break;
                }
            }
            else
            {
                switch (Thread.CurrentThread.CurrentCulture.ToString())
                {
                    case "tr-TR":

                        Global.Language = Global.LanguageType.Turkish;
                        dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);
                        MesnetSettings.WriteSetting("language", "tr-TR");

                        break;

                    default:

                        Global.Language = Global.LanguageType.English;
                        dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);
                        MesnetSettings.WriteSetting("language", "en-EN");

                        break;
                }
            }
            App.Current.Resources.MergedDictionaries.Add(dict);

            if (MesnetSettings.IsSettingExists("calculationtype"))
            {
                switch (MesnetSettings.ReadSetting("calculationtype"))
                {
                    case "singlethreaded":

                        Global.Calculation = Global.CalculationType.SingleThreaded;

                        break;

                    case "multithreaded":

                        Global.Calculation = Global.CalculationType.MultiThreaded;

                        break;
                }
            }
            else
            {
                Global.Calculation = Global.CalculationType.SingleThreaded;
                MesnetSettings.WriteSetting("calculationtype", "singlethreaded");
            }
        }

        private void MinSize(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);
            textBlock.Width = formattedText.Width;
            textBlock.Height = formattedText.Height;
        }

        private void RotateAround(TextBlock textBlock, Beam beam)
        {
            var rotate = new RotateTransform();
            rotate.CenterX = textBlock.Width / 2;
            rotate.CenterY = textBlock.Height / 2;
            rotate.Angle = -beam.Angle;
            textBlock.RenderTransform = rotate;
        }

        public static string AssociationPath = null;
    }
}
