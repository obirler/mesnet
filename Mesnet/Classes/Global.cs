using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using Mesnet.Classes.Math;
using Mesnet.Classes.Tools;
using Mesnet.Properties;
using Mesnet.Xaml.User_Controls;

namespace Mesnet.Classes
{
    public static class Global
    {
        public static List<object> objects = new List<object>();

        public static double maxmoment = Double.MinValue;

        public static double minmoment = Double.MinValue;

        public static double maxload = Double.MinValue;

        public static double maxdeflection = Double.MinValue;

        public static double maxstress = Double.MinValue;
            
        /// <summary>
        /// Sets the language of the application using system language.
        /// </summary>
        public static void SetLanguageDictionary()
        {
            if (App.Current.Resources.MergedDictionaries.Count != 0)
            {
                App.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "tr-TR":

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);
                    Settings.Default.language = "tr-TR";

                    break;

                default:

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);
                    Settings.Default.language = "en-EN";

                    break;
            }
            Settings.Default.Save();
            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        /// <summary>
        /// Sets the language of the application to the given language.
        /// </summary>
        /// <param name="lang">The desired language.</param>
        public static void SetLanguageDictionary(string lang)
        {
            if (App.Current.Resources.MergedDictionaries.Count != 0)
            {
                App.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "tr-TR":

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageTr.xaml", UriKind.Relative);
                    Settings.Default.language = "tr-TR";

                    break;

                default:

                    dict.Source = new Uri(@"..\Xaml\Resources\LanguageEn.xaml", UriKind.Relative);
                    Settings.Default.language = "en-EN";

                    break;
            }
            Settings.Default.Save();
            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        /// <summary>
        /// Gets the string by key from the current application language.
        /// </summary>
        /// <param name="key">The key of the desired string.</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return App.Current.Resources.MergedDictionaries[0][key].ToString();
        }

        public static int AddObject(object obj)
        {
            if (!objects.Contains(obj))
            {
                objects.Add(obj);
                return objects.IndexOf(obj);
            }
            else
            {
                MyDebug.WriteWarning("Global : AddObject", "the object already added!");
                return -1;
            }
        }

        public static Beam GetBeam(string Name)
        {
            foreach (var item in objects)
            {
                switch (item.GetType().Name)
                {
                    case "Beam":

                        var beam = item as Beam;

                        if (beam.Name == Name)
                        {
                            return beam;
                        }

                        break;                       
                }
            }
            return null;
        }

        public static object GetObject(int id)
        {
            return objects[id];
        }

        public static void SetDecimalSeperator()
        {
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public static double Scale = 1;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            None
        }

        public enum FunctionType
        {
            Fixed,
            Variable
        }

        public enum SupportType
        {
            BasicSupoort,
            SlidingSupport,
            LeftFixedSupport,
            RightFixedSupport
        }

        public enum CalculationType
        {
            SingleThreaded,
            MultiThreaded
        }

        public enum ObjectType
        {
            Beam,
            LeftFixedSupport,
            RightFixedSupport,
            BasicSupport,
            SlidingSupport,
            None
        }

        public static ObjectType GetObjectType(object obj)
        {
            switch (obj.GetType().Name)
            {
                case "Beam":
                    return ObjectType.Beam;

                case "BasicSupport":
                    return ObjectType.BasicSupport;

                case "LeftFixedSupport":
                    return ObjectType.LeftFixedSupport;

                case "RightFixedSupport":
                    return ObjectType.RightFixedSupport;

                case "SlidingSupport":
                    return ObjectType.SlidingSupport;
            }

            return ObjectType.None;
        }

        public static void WritePPolytoConsole(string message, PiecewisePoly ppoly)
        {
            foreach (Poly poly in ppoly)
            {
                MyDebug.WriteInformation("WritePPolytoConsole", message + " : " + poly.ToString() +" , " + poly.StartPoint + " <= x <= " + poly.EndPoint);
            }
        }

        public static int BeamCount = 0;

        public static int SupportCount = 0;

        public static double SimpsonStep = 0.0001;

        public struct Func
        {
            public int id;
            public double xposition;
            public double yposition;
        }

        public static List<string> LogList = new List<string>();

        public static CalculationType Calculation = CalculationType.MultiThreaded;
    }
}
