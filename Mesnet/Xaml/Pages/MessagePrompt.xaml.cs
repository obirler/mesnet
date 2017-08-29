using Mesnet.Classes;
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

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for MessagePrompt.xaml
    /// </summary>
    public partial class MessagePrompt : Window
    {
        public MessagePrompt(string message)
        {
            InitializeComponent();
            Message.Text = message;
        }

        public Global.DialogResult Result = Global.DialogResult.None;

        private void yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Yes;
            DialogResult = true;
        }

        private void nobtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.No;
            DialogResult = true;
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Cancel;
            DialogResult = true;
        }
    }
}
