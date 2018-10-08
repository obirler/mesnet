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
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using Mesnet.Classes.Tools;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for VersionDownloader.xaml
    /// </summary>
    public partial class VersionDownloader : Window
    {
        public VersionDownloader(MesnetVersion mesnetversion, string path)
        {
            InitializeComponent();
            this.version.Text = mesnetversion.Version;
            FilePath = path;
            _url = Config.ServerUrl + mesnetversion.Url;
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
