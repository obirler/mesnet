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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Mesnet.Classes;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for CrossSolve.xaml
    /// </summary>
    public partial class CrossSolve : Window
    {
        public CrossSolve(MainWindow mw)
        {
            _mw = mw;
            InitializeComponent();

            timer.Tick += timer_Tick;

            timer.Interval = TimeSpan.FromMilliseconds(10);

            bw.DoWork += bw_DoWork;
            bw.WorkerReportsProgress = true;
            bw3.DoWork += bw3LastProcess;

            timer.Start();
        }

        private MainWindow _mw;

        private static Mutex mutex = new Mutex();

        DispatcherTimer timer = new DispatcherTimer();

        BackgroundWorker bw = new BackgroundWorker();

        BackgroundWorker bw3 = new BackgroundWorker();

        private List<Beam> QueueList = new List<Beam>();

        private List<BackgroundWorker> bwlist = new List<BackgroundWorker>();

        private double calculated = 0;

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SetDecimalSeperator();

            SimpsonStep = 0.0001;

            if (Logger.IsClosed())
            {
                Logger.InitializeLogger();
            }
            MyDebug.WriteInformation("bw_DoWork", "************************************************************Cross Solve Algorithm Started****************************************");

            Logger.WriteLine("**************************Cross Solve Algorithm Started****************************");

            Logger.NextLine();

            if (BeamCount > 1)
            {
                switch (Calculation)
                {
                    case CalculationType.SingleThreaded:

                        foreach (var item in objects)
                        {
                            if (item.GetType().Name == "Beam")
                            {
                                Beam beam = (Beam)item;
                                beam.CrossCalculate();
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    calculated++;
                                    progress.Value = calculated / BeamCount * 100;
                                    progress.UpdateLayout();
                                    status.Text = "Calculating " + beam.Name;
                                }));                               
                            }
                        }

                        bw3.RunWorkerAsync();

                        break;

                    case CalculationType.MultiThreaded:

                        foreach (var item in objects)
                        {
                            if (item.GetType().Name == "Beam")
                            {
                                Beam beam = (Beam)item;
                                QueueList.Add(beam);
                            }
                        }

                        Thread.Sleep(700);

                        for (int i = 0; i < Environment.ProcessorCount; i++)
                        {
                            if (QueueList.Count > 0)
                            {
                                BackgroundWorker bw = new BackgroundWorker();
                                bw.DoWork += bwbeam_DoWork;
                                bwlist.Add(bw);
                                bw.RunWorkerAsync(i);
                            }
                        }

                        break;
                }
            }
            else
            {
                foreach (var item in objects)
                {
                    switch (item.GetType().Name)
                    {
                        case "Beam":

                            Beam beam = (Beam)item;
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                calculated++;
                                progress.Value = calculated / BeamCount * 100;
                                progress.UpdateLayout();
                                status.Text = "Calculating " + beam.Name;
                            }));
                            beam.ClapeyronCalculate();
                            beam.CalculateDeflection();
                            maxmoment = beam.MaxMoment;
                            minmoment = beam.MinMoment;

                            bw3.RunWorkerAsync();

                            break;
                    }
                }

            }
        }

        private void bwbeam_DoWork(object senderbw, DoWorkEventArgs ebw)
        {
            int threadnumber = (int)ebw.Argument;
            MyDebug.WriteInformation("BACKGROUNDWORKER " + threadnumber, " started to work");
            SetDecimalSeperator();
            Beam cachebeam;
            while (QueueList.Count > 0)
            {
                mutex.WaitOne();
                cachebeam = QueueList.First();
                if (cachebeam != null)
                {
                    QueueList.Remove(cachebeam);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        calculated++;
                        progress.Value = calculated/BeamCount*100;
                        progress.UpdateLayout();
                        status.Text = "Calculating " + cachebeam.Name;
                    }));
                    mutex.ReleaseMutex();
                    cachebeam.CrossCalculate();
                }
                else
                {
                    mutex.ReleaseMutex();
                }                
            }

            foreach (var worker in bwlist)
            {
                if (worker != senderbw)
                {
                    if (worker.IsBusy)
                    {
                        return;
                    }
                }
            }

            bw3.RunWorkerAsync();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            bw.RunWorkerAsync();
        }

        private void bw3LastProcess(object sender, DoWorkEventArgs e)
        {
            if (BeamCount > 1)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SetDecimalSeperator();
                    _mw.WriteCarryoverFactors();
                    var obj = _mw.MaxMomentSupport();
                    _mw.IndexAll(obj);
                    _mw.CrossLoop();
                    _mw.UpdateBeams();
                    _mw.UpdateAllTree();
                    _mw.UpdateAllSupportTree();
                    _mw.ShowMoments();
                    DialogResult = true;
                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SetDecimalSeperator();
                    _mw.UpdateBeam();
                    _mw.ShowMoments();
                    _mw.UpdateAllTree();
                    _mw.UpdateAllSupportTree();
                    DialogResult = true;
                }));
            }
        }
    }
}
