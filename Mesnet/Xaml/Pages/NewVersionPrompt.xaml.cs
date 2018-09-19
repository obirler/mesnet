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
using System.Windows.Shapes;
using Mesnet.Classes;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for NewVersionPrompt.xaml
    /// </summary>
    public partial class NewVersionPrompt : Window
    {
        public NewVersionPrompt(string versionnumber)
        {
            InitializeComponent();
            version.Text = versionnumber;
        }

        public Global.DialogResult Result = Global.DialogResult.None;

        public bool DontAskToUpdate = false;

        private void yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Yes;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void nobtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.No;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Cancel;
            DontAskToUpdate = (bool)dontshowcbx.IsChecked;
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Result = Global.DialogResult.Yes;
                DontAskToUpdate = (bool)dontshowcbx.IsChecked;
                DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                Result = Global.DialogResult.No;
                DontAskToUpdate = (bool)dontshowcbx.IsChecked;
                DialogResult = true;
            }
        }
    }
}
