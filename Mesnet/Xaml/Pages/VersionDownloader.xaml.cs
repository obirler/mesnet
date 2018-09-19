using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Mesnet.Classes;
using Mesnet.Classes.Tools;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for VersionDownloader.xaml
    /// </summary>
    public partial class VersionDownloader : Window
    {
        public VersionDownloader(MesnetVersion mesnetversion)
        {
            InitializeComponent();
            this.version.Text = mesnetversion.Version;
            _url = Global.ServerUrl + mesnetversion.Url;
            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _bw = new BackgroundWorker();
            _bw.DoWork += BwOnDoWork;
            _webClient=new WebClient();
            _webClient.DownloadProgressChanged += WcOnDownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
            _timer.Start();
        }

        private void BwOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {           
            FilePath = System.IO.Path.GetFileName(_url);
            _webClient.DownloadFileAsync(new System.Uri(_url), FilePath);
        }
       
        private string _url;

        private DispatcherTimer _timer;

        private BackgroundWorker _bw;

        private WebClient _webClient;

        public string FilePath;

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();
            _bw.RunWorkerAsync();
        }

        private void WcOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                progresspercent.Minimum = 0;
                double receive = Convert.ToDouble(e.BytesReceived);
                double total = Convert.ToDouble(e.TotalBytesToReceive);
                int percentage = Convert.ToInt32(receive / total * 100);
                progresstxt.Text = "%" + percentage;
                //MesnetDebug.WriteInformation("Download percent : " + percentage);
                progresspercent.Value = percentage;
            }));          
        }
        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            Dispatcher.BeginInvoke(new Action(()=>
            {
                DialogResult = true;
            }));           
        }
    }
}
