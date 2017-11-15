using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Mesnet.Classes.Math;
using static Mesnet.Classes.Global;
using Mesnet.Xaml.User_Controls;

namespace Mesnet.Classes.Tools
{
    class TestCases
    {
        public TestCases(MainWindow mw)
        {
            _mw = mw;
        }

        private MainWindow _mw;

        public void RegisterTests()
        {
            RegisterIssue6Test();

            RegisterTestCase();

            RegisterTestCase2();

            RegisterTestCase3();

            RegisterTestCase4();

            RegisterTestCase5();

            RegisterDevTest();

            RegisterDevTest2();

            RegisterStressTest();

            RegisterStressTest2();

            RegisterInertiaTest();

            RegisterInertiaTest();

            RegisterReverseTest();

            RegisterReverseTest();

            RegisterReverseLoadTest();

            RegisterIntenseReverseTest();

            RegisterGem513OdevTest();
        }

        //inertia for test purpose in [cm^4]
        private double _testi = 70000;

        private double _testdim = 30;

        private void RegisterIssue6Test()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Issue 6 Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(2);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                beam1.AddTopLeft(_mw.canvas, 10200, 9700);

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);

                var basicsupport2 = new BasicSupport(_mw.canvas);
                basicsupport2.AddBeam(beam1, Global.Direction.Left);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("-30x+30", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ///////////////////////////////////////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 1.5);
                beam2.AddElasticity(200);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport3 = new BasicSupport(_mw.canvas);
                basicsupport3.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("30", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterTestCase()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Test Case";
            menuitem.Click += (sender, args) =>
            {
                /////////////////////////////// Beam 1 (GA) /////////////////////////////
                var beam1 = new Beam(16);
                beam1.AddElasticity(205);
                var polies1 = new List<Poly>();
                polies1.Add(new Poly("3", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(polies1));
                beam1.AddTopLeft(_mw.canvas, 10100, 9400);

                var slidingsupport = new BasicSupport(_mw.canvas);
                slidingsupport.AddBeam(beam1, Global.Direction.Left);

                var slidingsupport1 = new BasicSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("10", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                /////////////////////////////// Beam 2 (AB) //////////////////////////////

                var beam2 = new Beam(_mw.canvas, 3);
                beam2.AddElasticity(205);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("3", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);
                beam2.SetAngleLeft(90);

                var slidingsupport2 = new BasicSupport(_mw.canvas);
                slidingsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("10+6.66666666667x", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////// Beam 3 (FB) /////////////////////////////

                var beam3 = new Beam(_mw.canvas, 16);
                beam3.AddElasticity(205);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly("5", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Right, beam2, Global.Direction.Right);

                var slidingsupport3 = new BasicSupport(_mw.canvas);
                slidingsupport3.AddBeam(beam3, Global.Direction.Left);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("30", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////// Beam 4 (BC) /////////////////////////////

                var beam4 = new Beam(_mw.canvas, 8);
                beam4.AddElasticity(205);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly("4", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam4.SetAngleLeft(90);

                var slidingsupport4 = new BasicSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("30+10x", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                /////////////////////////////// Beam 5 (CD) /////////////////////////////

                var beam5 = new Beam(8);
                beam5.AddElasticity(205);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly("20", 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.AddTopLeft(_mw.canvas, 5000, 5000);

                beam5.Connect(Global.Direction.Right, beam4, Global.Direction.Right);

                var slidingsupport5 = new BasicSupport(_mw.canvas);
                slidingsupport5.AddBeam(beam5, Global.Direction.Left);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("-110", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                /////////////////////////////// Beam 6 (DE) /////////////////////////////

                var beam6 = new Beam(_mw.canvas, 8);
                beam6.AddElasticity(205);
                var polies6 = new List<Poly>();
                polies6.Add(new Poly("20", 0, beam6.Length));
                beam6.AddInertia(new PiecewisePoly(polies6));
                beam6.Connect(Global.Direction.Left, beam5, Global.Direction.Left);
                beam6.SetAngleLeft(180);

                var slidingsupport6 = new BasicSupport(_mw.canvas);
                slidingsupport6.AddBeam(beam6, Global.Direction.Right);

                var loadpolies6 = new List<Poly>();
                loadpolies6.Add(new Poly("110", 0, beam6.Length));
                var ppoly6 = new PiecewisePoly(loadpolies6);
                var load6 = new DistributedLoad(ppoly6, beam6.Length);
                beam6.AddLoad(load6, Global.Direction.Up);

                /////////////////////////////// Beam 7 (EF) /////////////////////////////

                var beam7 = new Beam(_mw.canvas, 8);
                beam7.AddElasticity(205);
                var polies7 = new List<Poly>();
                polies7.Add(new Poly("4", 0, beam7.Length));
                beam7.AddInertia(new PiecewisePoly(polies7));
                beam7.Connect(Global.Direction.Left, beam6, Global.Direction.Right);
                beam7.SetAngleLeft(-90);
                beam7.CircularConnect(Global.Direction.Right, beam3, Global.Direction.Left);

                var loadpolies7 = new List<Poly>();
                loadpolies7.Add(new Poly("110-10x", 0, beam7.Length));
                var ppoly7 = new PiecewisePoly(loadpolies7);
                var load7 = new DistributedLoad(ppoly7, beam7.Length);
                beam7.AddLoad(load7, Global.Direction.Up);

                /////////////////////////////// Beam 8 (FG) /////////////////////////////

                var beam8 = new Beam(_mw.canvas, 3);
                beam8.AddElasticity(205);
                var polies8 = new List<Poly>();
                polies8.Add(new Poly("3", 0, beam8.Length));
                beam8.AddInertia(new PiecewisePoly(polies8));
                beam8.Connect(Global.Direction.Left, beam3, Global.Direction.Left);
                beam8.SetAngleLeft(-90);
                beam8.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Left);

                var loadpolies8 = new List<Poly>();
                loadpolies8.Add(new Poly("30-6.66666666667x", 0, beam8.Length));
                var ppoly8 = new PiecewisePoly(loadpolies8);
                var load8 = new DistributedLoad(ppoly8, beam8.Length);
                beam8.AddLoad(load8, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterTestCase2()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Test Case 2";
            menuitem.Click += (sender, args) =>
            {
                /////////////////////////////// Beam 1 (GA) /////////////////////////////
                var beam1 = new Beam(16);
                beam1.AddElasticity(205);
                var polies1 = new List<Poly>();
                polies1.Add(new Poly((3 * _testi).ToString(), 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(polies1));
                beam1.AddTopLeft(_mw.canvas, 10100, 8600);

                var slidingsupport = new BasicSupport(_mw.canvas);
                slidingsupport.AddBeam(beam1, Global.Direction.Left);

                var slidingsupport1 = new BasicSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("10", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                beam1.PerformStressAnalysis = true;
                var epolies = new List<Poly>();
                epolies.Add(new Poly((3 * _testdim).ToString(), 0, beam1.Length));
                beam1.AddE(new PiecewisePoly(epolies));
                var dpolies = new List<Poly>();
                dpolies.Add(new Poly((3 * 2 * _testdim).ToString(), 0, beam1.Length));
                beam1.AddD(new PiecewisePoly(dpolies));
                beam1.MaxAllowableStress = 150;

                /////////////////////////////// Beam 2 (AB) //////////////////////////////

                var beam2 = new Beam(_mw.canvas, 3);
                beam2.AddElasticity(205);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly((3 * _testi).ToString(), 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);
                beam2.SetAngleLeft(90);

                var slidingsupport2 = new BasicSupport(_mw.canvas);
                slidingsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("10+6.66666666667x", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                beam2.PerformStressAnalysis = true;
                var epolies2 = new List<Poly>();
                epolies2.Add(new Poly((3 * _testdim).ToString(), 0, beam2.Length));
                beam2.AddE(new PiecewisePoly(epolies2));
                var dpolies2 = new List<Poly>();
                dpolies2.Add(new Poly((3 * 2 * _testdim).ToString(), 0, beam2.Length));
                beam2.AddD(new PiecewisePoly(dpolies2));
                beam2.MaxAllowableStress = 150;

                /////////////////////////////// Beam 3 (FB) /////////////////////////////

                var beam3 = new Beam(_mw.canvas, 16);
                beam3.AddElasticity(205);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly((5 * _testi).ToString(), 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Right, beam2, Global.Direction.Right);

                var slidingsupport3 = new BasicSupport(_mw.canvas);
                slidingsupport3.AddBeam(beam3, Global.Direction.Left);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("30", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                beam3.PerformStressAnalysis = true;
                var epolies3 = new List<Poly>();
                epolies3.Add(new Poly((5 * _testdim).ToString(), 0, beam3.Length));
                beam3.AddE(new PiecewisePoly(epolies3));
                var dpolies3 = new List<Poly>();
                dpolies3.Add(new Poly((5 * 2 * _testdim).ToString(), 0, beam3.Length));
                beam3.AddD(new PiecewisePoly(dpolies3));
                beam3.MaxAllowableStress = 150;

                /////////////////////////////// Beam 4 (BC) /////////////////////////////

                var beam4 = new Beam(_mw.canvas, 8);
                beam4.AddElasticity(205);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly((4 * _testi).ToString(), 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam4.SetAngleLeft(90);

                var slidingsupport4 = new BasicSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("30+10x", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                beam4.PerformStressAnalysis = true;
                var epolies4 = new List<Poly>();
                epolies4.Add(new Poly((4 * _testdim).ToString(), 0, beam4.Length));
                beam4.AddE(new PiecewisePoly(epolies4));
                var dpolies4 = new List<Poly>();
                dpolies4.Add(new Poly((4 * 2 * _testdim).ToString(), 0, beam4.Length));
                beam4.AddD(new PiecewisePoly(dpolies4));
                beam4.MaxAllowableStress = 150;

                /////////////////////////////// Beam 5 (CD) /////////////////////////////

                var beam5 = new Beam(_mw.canvas, 8);
                beam5.AddElasticity(205);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly((20 * _testi).ToString(), 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam5.SetAngleLeft(180);

                var slidingsupport5 = new BasicSupport(_mw.canvas);
                slidingsupport5.AddBeam(beam5, Global.Direction.Right);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("110", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                beam5.PerformStressAnalysis = true;
                var epolies5 = new List<Poly>();
                epolies5.Add(new Poly((20 * _testdim).ToString(), 0, beam5.Length));
                beam5.AddE(new PiecewisePoly(epolies5));
                var dpolies5 = new List<Poly>();
                dpolies5.Add(new Poly((20 * 2 * _testdim).ToString(), 0, beam5.Length));
                beam5.AddD(new PiecewisePoly(dpolies5));
                beam5.MaxAllowableStress = 150;

                /////////////////////////////// Beam 6 (DE) /////////////////////////////

                var beam6 = new Beam(_mw.canvas, 8);
                beam6.AddElasticity(205);
                var polies6 = new List<Poly>();
                polies6.Add(new Poly((20 * _testi).ToString(), 0, beam6.Length));
                beam6.AddInertia(new PiecewisePoly(polies6));
                beam6.Connect(Global.Direction.Left, beam5, Global.Direction.Right);
                beam6.SetAngleLeft(180);

                var slidingsupport6 = new BasicSupport(_mw.canvas);
                slidingsupport6.AddBeam(beam6, Global.Direction.Right);

                var loadpolies6 = new List<Poly>();
                loadpolies6.Add(new Poly("110", 0, beam6.Length));
                var ppoly6 = new PiecewisePoly(loadpolies6);
                var load6 = new DistributedLoad(ppoly6, beam6.Length);
                beam6.AddLoad(load6, Global.Direction.Up);

                beam6.PerformStressAnalysis = true;
                var epolies6 = new List<Poly>();
                epolies6.Add(new Poly((20 * _testdim).ToString(), 0, beam6.Length));
                beam6.AddE(new PiecewisePoly(epolies6));
                var dpolies6 = new List<Poly>();
                dpolies6.Add(new Poly((20 * 2 * _testdim).ToString(), 0, beam6.Length));
                beam6.AddD(new PiecewisePoly(dpolies6));
                beam6.MaxAllowableStress = 150;

                /////////////////////////////// Beam 7 (EF) /////////////////////////////

                var beam7 = new Beam(_mw.canvas, 8);
                beam7.AddElasticity(205);
                var polies7 = new List<Poly>();
                polies7.Add(new Poly((4 * _testi).ToString(), 0, beam7.Length));
                beam7.AddInertia(new PiecewisePoly(polies7));
                beam7.Connect(Global.Direction.Left, beam6, Global.Direction.Right);
                beam7.SetAngleLeft(-90);
                beam7.CircularConnect(Global.Direction.Right, beam3, Global.Direction.Left);

                var loadpolies7 = new List<Poly>();
                loadpolies7.Add(new Poly("110-10x", 0, beam7.Length));
                var ppoly7 = new PiecewisePoly(loadpolies7);
                var load7 = new DistributedLoad(ppoly7, beam7.Length);
                beam7.AddLoad(load7, Global.Direction.Up);

                beam7.PerformStressAnalysis = true;
                var epolies7 = new List<Poly>();
                epolies7.Add(new Poly((4 * _testdim).ToString(), 0, beam7.Length));
                beam7.AddE(new PiecewisePoly(epolies7));
                var dpolies7 = new List<Poly>();
                dpolies7.Add(new Poly((4 * 2 * _testdim).ToString(), 0, beam7.Length));
                beam7.AddD(new PiecewisePoly(dpolies7));
                beam7.MaxAllowableStress = 150;

                /////////////////////////////// Beam 8 (FG) /////////////////////////////

                var beam8 = new Beam(_mw.canvas, 3);
                beam8.AddElasticity(205);
                var polies8 = new List<Poly>();
                polies8.Add(new Poly((3 * _testi).ToString(), 0, beam8.Length));
                beam8.AddInertia(new PiecewisePoly(polies8));
                beam8.Connect(Global.Direction.Left, beam3, Global.Direction.Left);
                beam8.SetAngleLeft(-90);
                beam8.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Left);

                var loadpolies8 = new List<Poly>();
                loadpolies8.Add(new Poly("30-6.66666666667x", 0, beam8.Length));
                var ppoly8 = new PiecewisePoly(loadpolies8);
                var load8 = new DistributedLoad(ppoly8, beam8.Length);
                beam8.AddLoad(load8, Global.Direction.Up);

                beam8.PerformStressAnalysis = true;
                var epolies8 = new List<Poly>();
                epolies8.Add(new Poly((3 * _testdim).ToString(), 0, beam8.Length));
                beam8.AddE(new PiecewisePoly(epolies8));
                var dpolies8 = new List<Poly>();
                dpolies8.Add(new Poly((3 * 2 * _testdim).ToString(), 0, beam8.Length));
                beam8.AddD(new PiecewisePoly(dpolies8));
                beam8.MaxAllowableStress = 150;

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterTestCase3()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Test Case 3";
            menuitem.Click += (sender, args) =>
            {
                /////////////////////////////// Beam 1 (AB) /////////////////////////////
                var beam1 = new Beam(6);
                beam1.AddElasticity(205);
                var polies1 = new List<Poly>();
                polies1.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(polies1));
                beam1.AddTopLeft(_mw.canvas, 10100, 9400);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport = new SlidingSupport(_mw.canvas);
                slidingsupport.AddBeam(beam1, Global.Direction.Left);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("20", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                /////////////////////////////// Beam 2 (Bc) //////////////////////////////

                var beam2 = new Beam(_mw.canvas, 6);
                beam2.AddElasticity(205);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport2 = new SlidingSupport(_mw.canvas);
                slidingsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("20", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////// Beam 3 (CD) /////////////////////////////

                var beam3 = new Beam(_mw.canvas, 6);
                beam3.AddElasticity(205);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly("1", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Left, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport3 = new SlidingSupport(_mw.canvas);
                slidingsupport3.AddBeam(beam3, Global.Direction.Right);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("20", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////// Beam 4 (DE) /////////////////////////////

                var beam4 = new Beam(_mw.canvas, 5);
                beam4.AddElasticity(205);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly("2", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam3, Global.Direction.Right);
                beam4.SetAngleLeft(90);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport4 = new SlidingSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("20", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                /////////////////////////////// Beam 5 (EF) /////////////////////////////

                var beam5 = new Beam(_mw.canvas, 10);
                beam5.AddElasticity(205);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly("5", 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam5.SetAngleLeft(90);

                beam5.core.MouseDown += _mw.core_MouseDown;
                beam5.core.MouseUp += _mw.core_MouseUp;
                beam5.core.MouseMove += _mw.core_MouseMove;
                beam5.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam5.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport5 = new SlidingSupport(_mw.canvas);
                slidingsupport5.AddBeam(beam5, Global.Direction.Right);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("20+8x", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                /////////////////////////////// Beam 6 (FG) /////////////////////////////

                var beam6 = new Beam(_mw.canvas, 9);
                beam6.AddElasticity(205);
                var polies6 = new List<Poly>();
                polies6.Add(new Poly("40", 0, beam6.Length));
                beam6.AddInertia(new PiecewisePoly(polies5));
                beam6.Connect(Global.Direction.Left, beam5, Global.Direction.Right);
                beam6.SetAngleLeft(180);

                beam6.core.MouseDown += _mw.core_MouseDown;
                beam6.core.MouseUp += _mw.core_MouseUp;
                beam6.core.MouseMove += _mw.core_MouseMove;
                beam6.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam6.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport6 = new SlidingSupport(_mw.canvas);
                slidingsupport6.AddBeam(beam6, Global.Direction.Right);

                var loadpolies6 = new List<Poly>();
                loadpolies6.Add(new Poly("100", 0, beam6.Length));
                var ppoly6 = new PiecewisePoly(loadpolies6);
                var load6 = new DistributedLoad(ppoly6, beam6.Length);
                beam6.AddLoad(load6, Global.Direction.Up);

                /////////////////////////////// Beam 7 (GH) /////////////////////////////

                var beam7 = new Beam(_mw.canvas, 9);
                beam7.AddElasticity(205);
                var polies7 = new List<Poly>();
                polies7.Add(new Poly("40", 0, beam7.Length));
                beam7.AddInertia(new PiecewisePoly(polies7));
                beam7.Connect(Global.Direction.Left, beam6, Global.Direction.Right);
                beam7.SetAngleLeft(180);

                beam7.core.MouseDown += _mw.core_MouseDown;
                beam7.core.MouseUp += _mw.core_MouseUp;
                beam7.core.MouseMove += _mw.core_MouseMove;
                beam7.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam7.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport7 = new SlidingSupport(_mw.canvas);
                slidingsupport7.AddBeam(beam7, Global.Direction.Right);

                var loadpolies7 = new List<Poly>();
                loadpolies7.Add(new Poly("100", 0, beam7.Length));
                var ppoly7 = new PiecewisePoly(loadpolies7);
                var load7 = new DistributedLoad(ppoly7, beam7.Length);
                beam7.AddLoad(load7, Global.Direction.Up);

                /////////////////////////////// Beam 8 (HI) /////////////////////////////

                var beam8 = new Beam(_mw.canvas, 10);
                beam8.AddElasticity(205);
                var polies8 = new List<Poly>();
                polies8.Add(new Poly("5", 0, beam8.Length));
                beam8.AddInertia(new PiecewisePoly(polies8));
                beam8.Connect(Global.Direction.Left, beam7, Global.Direction.Right);
                beam8.SetAngleLeft(-90);

                beam8.core.MouseDown += _mw.core_MouseDown;
                beam8.core.MouseUp += _mw.core_MouseUp;
                beam8.core.MouseMove += _mw.core_MouseMove;
                beam8.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam8.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport8 = new SlidingSupport(_mw.canvas);
                slidingsupport8.AddBeam(beam8, Global.Direction.Right);

                var loadpolies8 = new List<Poly>();
                loadpolies8.Add(new Poly("100-8x", 0, beam8.Length));
                var ppoly8 = new PiecewisePoly(loadpolies8);
                var load8 = new DistributedLoad(ppoly8, beam8.Length);
                beam8.AddLoad(load8, Global.Direction.Up);

                /////////////////////////////// Beam 9 (IA) /////////////////////////////

                var beam9 = new Beam(_mw.canvas, 5);
                beam9.AddElasticity(205);
                var polies9 = new List<Poly>();
                polies9.Add(new Poly("2", 0, beam9.Length));
                beam9.AddInertia(new PiecewisePoly(polies9));
                beam9.Connect(Global.Direction.Left, beam8, Global.Direction.Right);
                beam9.SetAngleLeft(-90);
                beam9.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Left);

                beam9.core.MouseDown += _mw.core_MouseDown;
                beam9.core.MouseUp += _mw.core_MouseUp;
                beam9.core.MouseMove += _mw.core_MouseMove;
                beam9.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam9.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var loadpolies9 = new List<Poly>();
                loadpolies9.Add(new Poly("20", 0, beam9.Length));
                var ppoly9 = new PiecewisePoly(loadpolies9);
                var load9 = new DistributedLoad(ppoly9, beam9.Length);
                beam9.AddLoad(load9, Global.Direction.Up);

                /////////////////////////////// Beam 10 (IJ) /////////////////////////////

                var beam10 = new Beam(_mw.canvas, 9);
                beam10.AddElasticity(205);
                var polies10 = new List<Poly>();
                polies10.Add(new Poly("4", 0, beam10.Length));
                beam10.AddInertia(new PiecewisePoly(polies10));
                beam10.Connect(Global.Direction.Left, beam8, Global.Direction.Right);

                beam10.core.MouseDown += _mw.core_MouseDown;
                beam10.core.MouseUp += _mw.core_MouseUp;
                beam10.core.MouseMove += _mw.core_MouseMove;
                beam10.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam10.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport10 = new SlidingSupport(_mw.canvas);
                slidingsupport10.AddBeam(beam10, Global.Direction.Right);

                /////////////////////////////// Beam 11 (JE) /////////////////////////////

                var beam11 = new Beam(_mw.canvas, 9);
                beam11.AddElasticity(205);
                var polies11 = new List<Poly>();
                polies11.Add(new Poly("4", 0, beam11.Length));
                beam11.AddInertia(new PiecewisePoly(polies11));
                beam11.Connect(Global.Direction.Left, beam10, Global.Direction.Right);
                beam11.CircularConnect(Global.Direction.Right, beam4, Global.Direction.Right);

                beam11.core.MouseDown += _mw.core_MouseDown;
                beam11.core.MouseUp += _mw.core_MouseUp;
                beam11.core.MouseMove += _mw.core_MouseMove;
                beam11.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam11.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                /////////////////////////////// Beam 12 (JG) /////////////////////////////

                var beam12 = new Beam(_mw.canvas, 10);
                beam12.AddElasticity(205);
                var polies12 = new List<Poly>();
                polies12.Add(new Poly("5", 0, beam12.Length));
                beam12.AddInertia(new PiecewisePoly(polies12));
                beam12.Connect(Global.Direction.Left, beam10, Global.Direction.Right);
                beam12.SetAngleLeft(90);
                beam12.CircularConnect(Global.Direction.Right, beam6, Global.Direction.Right);

                beam12.core.MouseDown += _mw.core_MouseDown;
                beam12.core.MouseUp += _mw.core_MouseUp;
                beam12.core.MouseMove += _mw.core_MouseMove;
                beam12.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam12.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterTestCase4()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Test Case 4";
            menuitem.Click += (sender, args) =>
            {
                /////////////////////////////// Beam 1 (GA) /////////////////////////////
                var beam1 = new Beam(5);
                beam1.AddElasticity(205);
                var polies1 = new List<Poly>();
                polies1.Add(new Poly("2", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(polies1));
                beam1.AddTopLeft(_mw.canvas, 10100, 9400);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var leftfixedsupport = new LeftFixedSupport(_mw.canvas);
                leftfixedsupport.AddBeam(beam1);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("20x", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                /////////////////////////////// Beam 2 (AB) //////////////////////////////

                var beam2 = new Beam(_mw.canvas, 2);
                beam2.AddElasticity(205);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("2", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport2 = new SlidingSupport(_mw.canvas);
                slidingsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("100-30x", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////// Beam 2 (AB) //////////////////////////////

                var beam3 = new Beam(_mw.canvas, 3);
                beam3.AddElasticity(205);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly("4", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Left, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport3 = new SlidingSupport(_mw.canvas);
                slidingsupport3.AddBeam(beam3, Global.Direction.Right);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("80", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////// Beam 4 (AB) //////////////////////////////

                var beam4 = new Beam(_mw.canvas, 3);
                beam4.AddElasticity(205);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly("6", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam3, Global.Direction.Right);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport4 = new SlidingSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("20x", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                /////////////////////////////// Beam 5 (AB) //////////////////////////////

                var beam5 = new Beam(_mw.canvas, 4);
                beam5.AddElasticity(205);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly("5", 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);

                beam5.core.MouseDown += _mw.core_MouseDown;
                beam5.core.MouseUp += _mw.core_MouseUp;
                beam5.core.MouseMove += _mw.core_MouseMove;
                beam5.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam5.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport5 = new SlidingSupport(_mw.canvas);
                slidingsupport5.AddBeam(beam5, Global.Direction.Right);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("80-30x", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterTestCase5()
        {

        }

        private void RegisterDevTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Development Test";
            menuitem.Click += (sender, args) =>
            {
                /////////////////////////////// Beam 1 (GA) /////////////////////////////
                var beam1 = new Beam(16);
                beam1.AddElasticity(200);
                var polies = new List<Poly>();
                polies.Add(new Poly("3", 0, 16));
                beam1.AddInertia(new PiecewisePoly(polies));
                beam1.AddTopLeft(_mw.canvas, 10050, 9900);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var leftfixedsupport = new LeftFixedSupport(_mw.canvas);

                leftfixedsupport.AddBeam(beam1);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);
                beam1.RightSide = slidingsupport1;

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("10", 0, 16));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                /////////////////////////////// Beam 2 (AB) //////////////////////////////

                var beam2 = new Beam(_mw.canvas, 3);
                beam2.AddElasticity(200);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("3", 0, 3));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);
                beam2.SetAngleLeft(90);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport2 = new SlidingSupport(_mw.canvas);
                slidingsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("10+6.66666666667x", 0, 3));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////// Beam 3 (FB) /////////////////////////////

                var beam3 = new Beam(_mw.canvas, 16);
                beam3.AddElasticity(200);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly("5", 0, 16));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Right, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var leftfixedsupport3 = new LeftFixedSupport(_mw.canvas);
                leftfixedsupport3.AddBeam(beam3);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("30", 0, 16));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////// Beam 4 (BC) /////////////////////////////

                var beam4 = new Beam(_mw.canvas, 8);
                beam4.AddElasticity(200);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly("4", 0, 8));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam4.SetAngleLeft(90);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport4 = new SlidingSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("30+10x", 0, 8));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                /////////////////////////////// Beam 5 (EC) /////////////////////////////

                var beam5 = new Beam(_mw.canvas, 8);
                beam5.AddElasticity(200);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly("20", 0, 8));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam5.SetAngleLeft(180);

                beam5.core.MouseDown += _mw.core_MouseDown;
                beam5.core.MouseUp += _mw.core_MouseUp;
                beam5.core.MouseMove += _mw.core_MouseMove;
                beam5.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam5.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var rightfixedsupport5 = new RightFixedSupport(_mw.canvas);
                rightfixedsupport5.AddBeam(beam5);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("110", 0, 8));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, 8);
                beam5.AddLoad(load5, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterDevTest2()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Development Test 2";
            menuitem.Click += (sender, args) =>
            {
                ///////////////////////////////Beam 1/////////////////////////////
                var beam1 = new Beam(4);
                beam1.AddElasticity(205);
                var polies = new List<Poly>();
                polies.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(polies));
                beam1.AddTopLeft(_mw.canvas, 10200, 9400);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport11 = new SlidingSupport(_mw.canvas);
                slidingsupport11.AddBeam(beam1, Global.Direction.Left);

                //var leftfixedsupoort1 = new LeftFixedSupport(_mw.canvas);
                //leftfixedsupoort1.AddBeam(beam1);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("10", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ///////////////////////////////Beam 2/////////////////////////////
                var beam2 = new Beam(_mw.canvas, 3);
                beam2.AddElasticity(205);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);
                beam2.SetAngleLeft(90);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport2 = new BasicSupport(_mw.canvas);
                basicsupport2.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("10+6.666666666667x", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                ///////////////////////////////Beam 3/////////////////////////////

                var beam3 = new Beam(_mw.canvas, 4);
                beam3.AddElasticity(205);
                var polies3 = new List<Poly>();
                polies3.Add(new Poly("2", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(polies3));
                beam3.Connect(Global.Direction.Right, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport3 = new SlidingSupport(_mw.canvas);
                slidingsupport3.AddBeam(beam3, Global.Direction.Left);

                //var leftfixedsupoort3= new LeftFixedSupport(_mw.canvas);
                //leftfixedsupoort3.AddBeam(beam3);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("30", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                ///////////////////////////////Beam 4/////////////////////////////

                var beam4 = new Beam(_mw.canvas, 5);
                beam4.AddElasticity(205);
                var polies4 = new List<Poly>();
                polies4.Add(new Poly("4", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(polies4));
                beam4.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam4.SetAngleLeft(90);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var slidingsupport4 = new SlidingSupport(_mw.canvas);
                slidingsupport4.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("30+14x", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                ///////////////////////////////Beam 5/////////////////////////////

                var beam5 = new Beam(_mw.canvas, 16);
                beam5.AddElasticity(205);
                var polies5 = new List<Poly>();
                polies5.Add(new Poly("40", 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(polies5));
                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam5.SetAngleLeft(180);

                beam5.core.MouseDown += _mw.core_MouseDown;
                beam5.core.MouseUp += _mw.core_MouseUp;
                beam5.core.MouseMove += _mw.core_MouseMove;
                beam5.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam5.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var rightfixedsupport5 = new RightFixedSupport(_mw.canvas);
                rightfixedsupport5.AddBeam(beam5);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("110", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterStressTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Stress Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(4);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("416.667+1250x+1250x^2+416.667x^3", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));
                if (beam1.MaxInertia > MaxInertia)
                {
                    MaxInertia = beam1.MaxInertia;
                }
                beam1.PerformStressAnalysis = true;
                var epolies = new List<Poly>();
                epolies.Add(new Poly("5+5x", 0, beam1.Length));
                beam1.AddE(new PiecewisePoly(epolies));
                var dpolies = new List<Poly>();
                dpolies.Add(new Poly("10+10x", 0, beam1.Length));
                beam1.AddD(new PiecewisePoly(dpolies));
                beam1.MaxAllowableStress = 150;
                beam1.AddTopLeft(_mw.canvas, 10200, 9700);
                //beam1.SetAngleCenter(90);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                /*
                var slidingsupport11 = new SlidingSupport(_mw.canvas);
                slidingsupport11.AddBeam(beam1, Direction.Left);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Direction.Right);

                */

                var leftsupport = new LeftFixedSupport(_mw.canvas);
                leftsupport.AddBeam(beam1);

                var rightsupport = new RightFixedSupport(_mw.canvas);
                rightsupport.AddBeam(beam1);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("50", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterStressTest2()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Stress Test 2";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(4);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("416.667+1250x+1250x^2+416.667x^3", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));
                if (beam1.MaxInertia > MaxInertia)
                {
                    MaxInertia = beam1.MaxInertia;
                }
                beam1.PerformStressAnalysis = true;
                var epolies = new List<Poly>();
                epolies.Add(new Poly("5+5x", 0, beam1.Length));
                beam1.AddE(new PiecewisePoly(epolies));
                var dpolies = new List<Poly>();
                dpolies.Add(new Poly("10+10x", 0, beam1.Length));
                beam1.AddD(new PiecewisePoly(dpolies));
                beam1.MaxAllowableStress = 150;
                beam1.AddTopLeft(_mw.canvas, 10200, 9700);
                //beam1.SetAngleCenter(90);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var leftsupport = new LeftFixedSupport(_mw.canvas);
                leftsupport.AddBeam(beam1);

                var slidingsupport1 = new SlidingSupport(_mw.canvas);
                slidingsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("50", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                //////////////////////////////////beam 2//////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 5);
                beam2.AddElasticity(200);
                var inertiapolies2 = new List<Poly>();
                inertiapolies2.Add(new Poly("416.667+1250x+1250x^2+416.667x^3", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(inertiapolies2));
                if (beam2.MaxInertia > MaxInertia)
                {
                    MaxInertia = beam2.MaxInertia;
                }
                beam2.PerformStressAnalysis = true;
                var epolies2 = new List<Poly>();
                epolies2.Add(new Poly("5+5x", 0, beam2.Length));
                beam2.AddE(new PiecewisePoly(epolies2));
                var dpolies2 = new List<Poly>();
                dpolies2.Add(new Poly("10+10x", 0, beam2.Length));
                beam2.AddD(new PiecewisePoly(dpolies));
                beam2.MaxAllowableStress = 100;

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var rightsupport2 = new RightFixedSupport(_mw.canvas);
                rightsupport2.AddBeam(beam2);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("40", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
        }

        private void RegisterInertiaTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Inertia Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(2);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("416.667+625x+312.5x^2+52.0833x^3", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.PerformStressAnalysis = true;
                var epolies = new List<Poly>();
                epolies.Add(new Poly("5+2.5x", 0, beam1.Length));
                beam1.AddE(new PiecewisePoly(epolies));
                var dpolies = new List<Poly>();
                dpolies.Add(new Poly("10+5x", 0, beam1.Length));
                beam1.AddD(new PiecewisePoly(dpolies));
                beam1.MaxAllowableStress = 150;
                beam1.AddTopLeft(_mw.canvas, 10200, 9700);

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var leftsupport = new LeftFixedSupport(_mw.canvas);
                leftsupport.AddBeam(beam1);

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("100-25x^2", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                //////////////////////////////////beam 2//////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 3);
                beam2.AddElasticity(200);
                var inertiapolies2 = new List<Poly>();
                inertiapolies2.Add(new Poly("3333.33-20000x+40000x^2-26666.7x^3", 0, 0.25));
                inertiapolies2.Add(new Poly("416.667", 0.25, 2.75));
                inertiapolies2.Add(new Poly("-416667+500000x-200000x^2+26666.7x^3", 2.75, 3));
                beam2.AddInertia(new PiecewisePoly(inertiapolies2));

                if (beam2.MaxInertia > MaxInertia)
                {
                    MaxInertia = beam2.MaxInertia;
                }
                beam2.PerformStressAnalysis = true;
                var epolies2 = new List<Poly>();
                epolies2.Add(new Poly("-20x+10", 0, 0.25));
                epolies2.Add(new Poly("5", 0.25, 2.75));
                epolies2.Add(new Poly("20x-50", 2.75, 3));
                beam2.AddE(new PiecewisePoly(epolies2));
                var dpolies2 = new List<Poly>();
                dpolies2.Add(new Poly("-40x+20", 0, 0.25));
                dpolies2.Add(new Poly("10", 0.25, 2.75));
                dpolies2.Add(new Poly("40x-100", 2.75, 3));
                beam2.AddD(new PiecewisePoly(dpolies2));
                beam2.MaxAllowableStress = 150;

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var rightsupport2 = new RightFixedSupport(_mw.canvas);
                rightsupport2.AddBeam(beam2);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("-10x+30", 1, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                var concloadlist = new List<KeyValuePair<double, double>>();
                var conc = new KeyValuePair<double, double>(0.5, 20);
                concloadlist.Add(conc);
                var concload2 = new ConcentratedLoad(concloadlist, beam2.Length);
                beam2.AddLoad(concload2, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterReverseTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Reverse Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(2);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;
                beam1.AddTopLeft(_mw.canvas, 10200, 9700);
                beam1.SetAngleCenter(180);

                /*beam1.InnerGeometry.drawrectcorners(5);

                beam1.OuterGeometry.drawrectcorners(3);*/

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);

                var basicsupport2 = new BasicSupport(_mw.canvas);
                basicsupport2.AddBeam(beam1, Global.Direction.Left);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("-30", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ///////////////////////////////////////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 1.5);
                beam2.AddElasticity(200);
                var polies2 = new List<Poly>();
                polies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(polies2));
                beam2.AddTopLeft(_mw.canvas, 10100, 9800);
                beam2.SetAngleCenter(90);

                var basicsupport3 = new BasicSupport(_mw.canvas);
                basicsupport3.AddBeam(beam2, Global.Direction.Right);

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Left);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("-30", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterReverseLoadTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Reverse Load Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(3);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                beam1.AddTopLeft(_mw.canvas, 10200, 9700);

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);

                var basicsupport2 = new BasicSupport(_mw.canvas);
                basicsupport2.AddBeam(beam1, Global.Direction.Left);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("40", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ////////////////////////////////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 2);
                beam2.AddElasticity(200);
                var inertiapolies2 = new List<Poly>();
                inertiapolies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(inertiapolies2));

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport3 = new BasicSupport(_mw.canvas);
                basicsupport3.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("-30x+30", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam3 = new Beam(_mw.canvas, 1.5);
                beam3.AddElasticity(200);
                var inertiapolies3 = new List<Poly>();
                inertiapolies3.Add(new Poly("1", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(inertiapolies3));

                beam3.Connect(Global.Direction.Left, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport4 = new BasicSupport(_mw.canvas);
                basicsupport4.AddBeam(beam3, Global.Direction.Right);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("20", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterIntenseReverseTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Intense Reverse Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(3);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("1", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                beam1.AddTopLeft(_mw.canvas, 10200, 9700);

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);

                var basicsupport2 = new BasicSupport(_mw.canvas);
                basicsupport2.AddBeam(beam1, Global.Direction.Left);

                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("40", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ////////////////////////////////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 2);
                beam2.AddElasticity(200);
                var inertiapolies2 = new List<Poly>();
                inertiapolies2.Add(new Poly("1", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(inertiapolies2));

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport3 = new BasicSupport(_mw.canvas);
                basicsupport3.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("30", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam3 = new Beam(_mw.canvas, 2);
                beam3.AddElasticity(200);
                var inertiapolies3 = new List<Poly>();
                inertiapolies3.Add(new Poly("1", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(inertiapolies3));

                beam3.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam3.SetAngleLeft(-90);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport4 = new BasicSupport(_mw.canvas);
                basicsupport4.AddBeam(beam3, Global.Direction.Right);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("-20", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam4 = new Beam(_mw.canvas, 2);
                beam4.AddElasticity(200);
                var inertiapolies4 = new List<Poly>();
                inertiapolies4.Add(new Poly("1", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(inertiapolies4));

                beam4.Connect(Global.Direction.Left, beam3, Global.Direction.Right);
                beam4.SetAngleLeft(-180);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport5 = new BasicSupport(_mw.canvas);
                basicsupport5.AddBeam(beam4, Global.Direction.Right);

                var loadpolies4 = new List<Poly>();
                loadpolies4.Add(new Poly("-20", 0, beam4.Length));
                var ppoly4 = new PiecewisePoly(loadpolies4);
                var load4 = new DistributedLoad(ppoly4, beam4.Length);
                beam4.AddLoad(load4, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam5 = new Beam(_mw.canvas, 3);
                beam5.AddElasticity(200);
                var inertiapolies5 = new List<Poly>();
                inertiapolies5.Add(new Poly("1", 0, beam5.Length));
                beam5.AddInertia(new PiecewisePoly(inertiapolies5));

                beam5.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam5.SetAngleLeft(-180);

                beam5.core.MouseDown += _mw.core_MouseDown;
                beam5.core.MouseUp += _mw.core_MouseUp;
                beam5.core.MouseMove += _mw.core_MouseMove;
                beam5.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam5.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport6 = new BasicSupport(_mw.canvas);
                basicsupport6.AddBeam(beam5, Global.Direction.Right);

                var loadpolies5 = new List<Poly>();
                loadpolies5.Add(new Poly("30", 0, beam5.Length));
                var ppoly5 = new PiecewisePoly(loadpolies5);
                var load5 = new DistributedLoad(ppoly5, beam5.Length);
                beam5.AddLoad(load5, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam6 = new Beam(_mw.canvas, 2);
                beam6.AddElasticity(200);
                var inertiapolies6 = new List<Poly>();
                inertiapolies6.Add(new Poly("1", 0, beam6.Length));
                beam6.AddInertia(new PiecewisePoly(inertiapolies6));

                beam6.Connect(Global.Direction.Left, beam5, Global.Direction.Right);
                beam6.SetAngleLeft(90);
                beam6.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Left);

                beam6.core.MouseDown += _mw.core_MouseDown;
                beam6.core.MouseUp += _mw.core_MouseUp;
                beam6.core.MouseMove += _mw.core_MouseMove;
                beam6.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam6.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var loadpolies6 = new List<Poly>();
                loadpolies6.Add(new Poly("-30", 0, beam6.Length));
                var ppoly6 = new PiecewisePoly(loadpolies6);
                var load6 = new DistributedLoad(ppoly6, beam6.Length);
                beam6.AddLoad(load6, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam7 = new Beam(_mw.canvas, 2);
                beam7.AddElasticity(200);
                var inertiapolies7 = new List<Poly>();
                inertiapolies7.Add(new Poly("1", 0, beam7.Length));
                beam7.AddInertia(new PiecewisePoly(inertiapolies7));

                beam7.Connect(Global.Direction.Left, beam4, Global.Direction.Right);
                beam7.SetAngleLeft(90);
                beam7.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Right);

                beam7.core.MouseDown += _mw.core_MouseDown;
                beam7.core.MouseUp += _mw.core_MouseUp;
                beam7.core.MouseMove += _mw.core_MouseMove;
                beam7.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam7.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var loadpolies7 = new List<Poly>();
                loadpolies7.Add(new Poly("-30", 0, beam7.Length));
                var ppoly7 = new PiecewisePoly(loadpolies7);
                var load7 = new DistributedLoad(ppoly7, beam7.Length);
                beam7.AddLoad(load7, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam8 = new Beam(_mw.canvas, 2);
                beam8.AddElasticity(200);
                var inertiapolies8 = new List<Poly>();
                inertiapolies8.Add(new Poly("1", 0, beam8.Length));
                beam8.AddInertia(new PiecewisePoly(inertiapolies8));

                beam8.Connect(Global.Direction.Left, beam2, Global.Direction.Right);
                beam8.SetAngleLeft(90);

                beam8.core.MouseDown += _mw.core_MouseDown;
                beam8.core.MouseUp += _mw.core_MouseUp;
                beam8.core.MouseMove += _mw.core_MouseMove;
                beam8.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam8.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport7 = new BasicSupport(_mw.canvas);
                basicsupport7.AddBeam(beam8, Global.Direction.Right);

                var loadpolies8 = new List<Poly>();
                loadpolies8.Add(new Poly("-30", 0, beam8.Length));
                var ppoly8 = new PiecewisePoly(loadpolies8);
                var load8 = new DistributedLoad(ppoly8, beam8.Length);
                beam8.AddLoad(load8, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam9 = new Beam(_mw.canvas, 2);
                beam9.AddElasticity(200);
                var inertiapolies9 = new List<Poly>();
                inertiapolies9.Add(new Poly("1", 0, beam9.Length));
                beam9.AddInertia(new PiecewisePoly(inertiapolies9));

                beam9.Connect(Global.Direction.Left, beam8, Global.Direction.Right);
                beam9.SetAngleLeft(180);

                beam9.core.MouseDown += _mw.core_MouseDown;
                beam9.core.MouseUp += _mw.core_MouseUp;
                beam9.core.MouseMove += _mw.core_MouseMove;
                beam9.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam9.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport8 = new BasicSupport(_mw.canvas);
                basicsupport8.AddBeam(beam9, Global.Direction.Right);

                var loadpolies9 = new List<Poly>();
                loadpolies9.Add(new Poly("-30", 0, beam8.Length));
                var ppoly9 = new PiecewisePoly(loadpolies9);
                var load9 = new DistributedLoad(ppoly9, beam9.Length);
                beam9.AddLoad(load9, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam10 = new Beam(_mw.canvas, 2);
                beam10.AddElasticity(200);
                var inertiapolies10 = new List<Poly>();
                inertiapolies10.Add(new Poly("1", 0, beam10.Length));
                beam10.AddInertia(new PiecewisePoly(inertiapolies10));

                beam10.Connect(Global.Direction.Left, beam9, Global.Direction.Right);
                beam10.SetAngleLeft(-90);
                beam10.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Right);

                beam10.core.MouseDown += _mw.core_MouseDown;
                beam10.core.MouseUp += _mw.core_MouseUp;
                beam10.core.MouseMove += _mw.core_MouseMove;
                beam10.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam10.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                //var basicsupport10 = new BasicSupport(_mw.canvas);
                //basicsupport10.AddBeam(beam10, Direction.Right);

                var loadpolies10 = new List<Poly>();
                loadpolies10.Add(new Poly("50", 0, beam10.Length));
                var ppoly10 = new PiecewisePoly(loadpolies10);
                var load10 = new DistributedLoad(ppoly10, beam10.Length);
                beam10.AddLoad(load10, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam11 = new Beam(_mw.canvas, 3);
                beam11.AddElasticity(200);
                var inertiapolies11 = new List<Poly>();
                inertiapolies11.Add(new Poly("1", 0, beam11.Length));
                beam11.AddInertia(new PiecewisePoly(inertiapolies11));

                beam11.Connect(Global.Direction.Left, beam9, Global.Direction.Right);
                beam11.SetAngleLeft(-180);

                beam11.core.MouseDown += _mw.core_MouseDown;
                beam11.core.MouseUp += _mw.core_MouseUp;
                beam11.core.MouseMove += _mw.core_MouseMove;
                beam11.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam11.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport11 = new BasicSupport(_mw.canvas);
                basicsupport11.AddBeam(beam11, Global.Direction.Right);

                var loadpolies11 = new List<Poly>();
                loadpolies11.Add(new Poly("20", 0, beam11.Length));
                var ppoly11 = new PiecewisePoly(loadpolies11);
                var load11 = new DistributedLoad(ppoly11, beam11.Length);
                beam11.AddLoad(load11, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam12 = new Beam(_mw.canvas, 2);
                beam12.AddElasticity(200);
                var inertiapolies12 = new List<Poly>();
                inertiapolies12.Add(new Poly("1", 0, beam12.Length));
                beam12.AddInertia(new PiecewisePoly(inertiapolies12));

                beam12.Connect(Global.Direction.Left, beam11, Global.Direction.Right);
                beam12.SetAngleLeft(-90);
                beam12.CircularConnect(Global.Direction.Right, beam1, Global.Direction.Left);

                beam12.core.MouseDown += _mw.core_MouseDown;
                beam12.core.MouseUp += _mw.core_MouseUp;
                beam12.core.MouseMove += _mw.core_MouseMove;
                beam12.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam12.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var loadpolies12 = new List<Poly>();
                loadpolies12.Add(new Poly("-30", 0, beam12.Length));
                var ppoly12 = new PiecewisePoly(loadpolies12);
                var load12 = new DistributedLoad(ppoly12, beam12.Length);
                beam12.AddLoad(load12, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }

        private void RegisterGem513OdevTest()
        {
            var menuitem = new MenuItem();
            menuitem.Header = "Gem 513 Odev Test";
            menuitem.Click += (sender, args) =>
            {
                var beam1 = new Beam(4);
                beam1.AddElasticity(200);
                var inertiapolies = new List<Poly>();
                inertiapolies.Add(new Poly("2", 0, beam1.Length));
                beam1.AddInertia(new PiecewisePoly(inertiapolies));

                beam1.core.MouseDown += _mw.core_MouseDown;
                beam1.core.MouseUp += _mw.core_MouseUp;
                beam1.core.MouseMove += _mw.core_MouseMove;
                beam1.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam1.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                beam1.AddTopLeft(_mw.canvas, 10200, 9700);
                beam1.SetAngleCenter(-90);

                var leftfixedsupport = new LeftFixedSupport(_mw.canvas);
                leftfixedsupport.AddBeam(beam1);

                var basicsupport1 = new BasicSupport(_mw.canvas);
                basicsupport1.AddBeam(beam1, Global.Direction.Right);
               
                var loadpolies1 = new List<Poly>();
                loadpolies1.Add(new Poly("5", 0, beam1.Length));
                var ppoly1 = new PiecewisePoly(loadpolies1);
                var load1 = new DistributedLoad(ppoly1, beam1.Length);
                beam1.AddLoad(load1, Global.Direction.Up);

                ////////////////////////////////////////////////////////////

                var beam2 = new Beam(_mw.canvas, 4);
                beam2.AddElasticity(200);
                var inertiapolies2 = new List<Poly>();
                inertiapolies2.Add(new Poly("4", 0, beam2.Length));
                beam2.AddInertia(new PiecewisePoly(inertiapolies2));

                beam2.Connect(Global.Direction.Left, beam1, Global.Direction.Right);

                beam2.core.MouseDown += _mw.core_MouseDown;
                beam2.core.MouseUp += _mw.core_MouseUp;
                beam2.core.MouseMove += _mw.core_MouseMove;
                beam2.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam2.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport3 = new BasicSupport(_mw.canvas);
                basicsupport3.AddBeam(beam2, Global.Direction.Right);

                var loadpolies2 = new List<Poly>();
                loadpolies2.Add(new Poly("5", 0, beam2.Length));
                var ppoly2 = new PiecewisePoly(loadpolies2);
                var load2 = new DistributedLoad(ppoly2, beam2.Length);
                beam2.AddLoad(load2, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam3 = new Beam(_mw.canvas, 4);
                beam3.AddElasticity(200);
                var inertiapolies3 = new List<Poly>();
                inertiapolies3.Add(new Poly("1", 0, beam3.Length));
                beam3.AddInertia(new PiecewisePoly(inertiapolies3));

                beam3.Connect(Global.Direction.Left, beam2, Global.Direction.Right);

                beam3.core.MouseDown += _mw.core_MouseDown;
                beam3.core.MouseUp += _mw.core_MouseUp;
                beam3.core.MouseMove += _mw.core_MouseMove;
                beam3.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam3.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var basicsupport4 = new BasicSupport(_mw.canvas);
                basicsupport4.AddBeam(beam3, Global.Direction.Right);

                var loadpolies3 = new List<Poly>();
                loadpolies3.Add(new Poly("10", 0, beam3.Length));
                var ppoly3 = new PiecewisePoly(loadpolies3);
                var load3 = new DistributedLoad(ppoly3, beam3.Length);
                beam3.AddLoad(load3, Global.Direction.Up);

                /////////////////////////////////////////////////////////////

                var beam4 = new Beam(_mw.canvas, 5);
                beam4.AddElasticity(200);
                var inertiapolies4 = new List<Poly>();
                inertiapolies4.Add(new Poly("2", 0, beam4.Length));
                beam4.AddInertia(new PiecewisePoly(inertiapolies4));

                beam4.Connect(Global.Direction.Left, beam3, Global.Direction.Right);
                beam4.SetAngleLeft(53.1301);

                beam4.core.MouseDown += _mw.core_MouseDown;
                beam4.core.MouseUp += _mw.core_MouseUp;
                beam4.core.MouseMove += _mw.core_MouseMove;
                beam4.startcircle.MouseDown += _mw.StartCircle_MouseDown;
                beam4.endcircle.MouseDown += _mw.EndCircle_MouseDown;

                var rightfixedsupport = new RightFixedSupport(_mw.canvas);
                rightfixedsupport.AddBeam(beam4);

                _mw.LoadsShown = true;
                _mw.loads.Header = GetString("hideloads");
                _mw.loads.IsEnabled = true;

                _mw.DisableTestMenus();

                _mw.UpdateAllSupportTree();

                _mw.UpdateAllBeamTree();
            };
            _mw.testmenu.Items.Add(menuitem);
        }
    }
}
