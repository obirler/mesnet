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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Mesnet.Classes.Math;
using Mesnet.Classes.Tools;
using Mesnet.Xaml.User_Controls;
using static Mesnet.Classes.Global;

namespace Mesnet.Classes.Ui
{
    public class TreeHandler
    {
        public TreeHandler(MainWindow mw)
        {
            _mw = mw;
            _beamtree = _mw.tree;
            _supporttree = _mw.supporttree;
        }

        private MainWindow _mw;

        private TreeView _beamtree;

        private TreeView _supporttree;

        public bool BeamTreeItemSelectedEventEnabled = true;

        public bool SupportTreeItemSelectedEventEnabled = true;

        #region Beam Tree Methods and Events

        /// <summary>
        /// Updates given beam in the beam tree view.
        /// </summary>
        /// <param name="beam">The beam.</param>
        public void UpdateBeamTree(Beam beam)
        {
            var beamitem = new TreeViewBeamItem(beam);
            bool exists = false;

            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    item.Items.Clear();
                    beamitem = item;
                    exists = true;
                    break;
                }
            }

            BeamItem bitem;

            if (!exists)
            {
                bitem = new BeamItem(beam);
                beamitem.Header = bitem;
                _beamtree.Items.Add(beamitem);
            }
            else
            {
                bitem = new BeamItem(beam);
                beamitem.Header = bitem;
            }

            if (beam.PerformStressAnalysis)
            {
                if (beam.Stress != null)
                {
                    if (beam.Stress.YMaxAbs >= beam.MaxAllowableStress)
                    {
                        bitem.SetCritical(true);
                    }
                    else
                    {
                        bitem.SetCritical(false);
                    }
                }
            }
            else
            {
                bitem.SetCritical(false);
            }

            beamitem.Selected += BeamTreeItemSelected;

            var arrowitem = new TreeViewItem();
            var arrowbutton = new ButtonItem();
            if (!beam.DirectionShown)
            {
                arrowbutton.SetName(GetString("showdirection"));
            }
            else
            {
                arrowbutton.SetName(GetString("hidedirection"));
            }
            arrowitem.Header = arrowbutton;
            arrowbutton.content.Click += _mw.arrow_Click;
            beamitem.Items.Add(arrowitem);

#if DEBUG
            var ideitem = new TreeViewItem();
            ideitem.Header = "Id : " + beam.Id;
            beamitem.Items.Add(ideitem);
#endif

            var lengthitem = new TreeViewItem();
            lengthitem.Header = new LengthItem(GetString("length") + " : " + beam.Length + " m");
            beamitem.Items.Add(lengthitem);

            var leftsideitem = new BeamSupportItem();

            if (beam.LeftSide != null)
            {
                string leftname = GetString("null");
                switch (GetObjectType(beam.LeftSide))
                {
                    case Global.ObjectType.LeftFixedSupport:
                        var ls = beam.LeftSide as LeftFixedSupport;
                        leftname = GetString("leftfixedsupport") + " " + ls.SupportId;
                        leftsideitem.Support = ls;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = beam.LeftSide as SlidingSupport;
                        leftname = GetString("slidingsupport") + " " + ss.SupportId;
                        leftsideitem.Support = ss;
                        break;

                    case Global.ObjectType.BasicSupport:
                        var bs = beam.LeftSide as BasicSupport;
                        leftname = GetString("basicsupport") + " " + bs.SupportId;
                        leftsideitem.Support = bs;
                        break;
                }
                leftsideitem.Header.Text = GetString("leftside") + " : " + leftname;
                beamitem.Items.Add(leftsideitem);
            }
            else
            {
                leftsideitem.Header.Text = GetString("leftside") + " : " + GetString("null");
                beamitem.Items.Add(leftsideitem);
            }

            var rightsideitem = new BeamSupportItem();

            if (beam.RightSide != null)
            {
                string rightname = GetString("null");
                switch (GetObjectType(beam.RightSide))
                {
                    case Global.ObjectType.RightFixedSupport:
                        var rs = beam.RightSide as RightFixedSupport;
                        rightname = GetString("rightfixedsupport") + " " + rs.SupportId;
                        rightsideitem.Support = rs;
                        break;

                    case Global.ObjectType.SlidingSupport:
                        var ss = beam.RightSide as SlidingSupport;
                        rightname = GetString("slidingsupport") + " " + ss.SupportId;
                        rightsideitem.Support = ss;
                        break;

                    case Global.ObjectType.BasicSupport:
                        var bs = beam.RightSide as BasicSupport;
                        rightname = GetString("basicsupport") + " " + bs.SupportId;
                        rightsideitem.Support = bs;
                        break;
                }
                rightsideitem.Header.Text = GetString("rightside") + " : " + rightname;
                beamitem.Items.Add(rightsideitem);
            }
            else
            {
                rightsideitem.Header.Text = GetString("rightside") + " : " + GetString("null");
                beamitem.Items.Add(rightsideitem);
            }

            var elasticityitem = new TreeViewItem();
            elasticityitem.Header = new ElasticityItem(GetString("elasticity") + " : " + beam.ElasticityModulus + " GPa");
            beamitem.Items.Add(elasticityitem);

            var inertiaitem = new TreeViewItem();

            inertiaitem.Header = new InertiaItem(GetString("inertia"));
            beamitem.Items.Add(inertiaitem);

            foreach (Poly inertiapoly in beam.Inertias)
            {
                var inertiachilditem = new TreeViewItem();
                inertiachilditem.Header = inertiapoly.GetString(4) + " ,  " + inertiapoly.StartPoint + " <= x <= " + inertiapoly.EndPoint;
                inertiaitem.Items.Add(inertiachilditem);
            }

            if (beam.ConcentratedLoads?.Count > 0)
            {
                var concloaditem = new TreeViewItem();
                concloaditem.Header = new ConcentratedLoadItem(GetString("concentratedloads"));
                beamitem.Items.Add(concloaditem);

                foreach (KeyValuePair<double, double> item in beam.ConcentratedLoads)
                {
                    var concloadchilditem = new TreeViewItem();
                    concloadchilditem.Header = System.Math.Round(item.Value, 4) + " ,  " + System.Math.Round(item.Key, 4) + " m";
                    concloaditem.Items.Add(concloadchilditem);
                }
            }

            if (beam.DistributedLoads?.Count > 0)
            {
                var distloaditem = new TreeViewItem();
                distloaditem.Header = new LoadItem(GetString("distributedloads"));
                beamitem.Items.Add(distloaditem);

                foreach (Poly distloadpoly in beam.DistributedLoads)
                {
                    var distloadchilditem = new TreeViewItem();
                    distloadchilditem.Header = distloadpoly.GetString(4) + " ,  " + distloadpoly.StartPoint + " <= x <= " + distloadpoly.EndPoint;
                    distloaditem.Items.Add(distloadchilditem);
                }
            }

            if (beam.FixedEndForce != null)
            {
                var forcetitem = new TreeViewItem();
                forcetitem.Header = new ForceItem(GetString("forcefunction"));
                beamitem.Items.Add(forcetitem);

                foreach (Poly forcepoly in beam.FixedEndForce)
                {
                    var forcechilditem = new TreeViewItem();
                    forcechilditem.Header = forcepoly.GetString(4) + " ,  " + forcepoly.StartPoint + " <= x <= " + forcepoly.EndPoint;
                    forcetitem.Items.Add(forcechilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                forcetitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + System.Math.Round(beam.FixedEndForce.Calculate(0), 4) + " kN";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + System.Math.Round(beam.FixedEndForce.Calculate(beam.Length), 4) + " kN";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + System.Math.Round(beam.FixedEndForce.Max, 4) + " kN";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + System.Math.Round(beam.FixedEndForce.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + System.Math.Round(beam.FixedEndForce.Min, 4) + " kN";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + System.Math.Round(beam.FixedEndForce.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var forceexplorer = new ForceExplorer();
                forceexplorer.xvalue.Text = "0";
                forceexplorer.funcvalue.Text = System.Math.Round(beam.FixedEndForce.Calculate(0), 4).ToString();
                forceexplorer.xvalue.TextChanged += _mw.fixedendforceexplorer_TextChanged;
                exploreritem.Header = forceexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.FixedEndMoment != null)
            {
                var momentitem = new TreeViewItem();
                momentitem.Header = new MomentItem(GetString("momentfunction"));
                beamitem.Items.Add(momentitem);

                foreach (Poly momentpoly in beam.FixedEndMoment)
                {
                    var momentchilditem = new TreeViewItem();
                    momentchilditem.Header = momentpoly.GetString(4) + " ,  " + momentpoly.StartPoint + " <= x <= " + momentpoly.EndPoint;
                    momentitem.Items.Add(momentchilditem);
                }

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                momentitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + System.Math.Round(beam.FixedEndMoment.Calculate(0), 4) + " kNm";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + System.Math.Round(beam.FixedEndMoment.Calculate(beam.Length), 4) + " kNm";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + System.Math.Round(beam.FixedEndMoment.Max, 4) + " kNm";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + System.Math.Round(beam.FixedEndMoment.MaxLocation, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + System.Math.Round(beam.FixedEndMoment.Min, 4) + " kNm";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + System.Math.Round(beam.FixedEndMoment.MinLocation, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var momentexplorer = new MomentExplorer();
                momentexplorer.xvalue.Text = "0";
                momentexplorer.funcvalue.Text = System.Math.Round(beam.FixedEndMoment.Calculate(0), 4).ToString();
                momentexplorer.xvalue.TextChanged += _mw.fixedendmomentexplorer_TextChanged;
                exploreritem.Header = momentexplorer;
                infoitem.Items.Add(exploreritem);
            }

            if (beam.PerformStressAnalysis && beam.Stress != null)
            {
                var stressitem = new TreeViewItem();
                stressitem.Header = new StressItem(GetString("stressdist"));
                beamitem.Items.Add(stressitem);

                var infoitem = new TreeViewItem();
                var info = new Information(GetString("information"));
                infoitem.Header = info;
                stressitem.Items.Add(infoitem);

                var leftside = new TreeViewItem();
                leftside.Header = GetString("leftside") + " : " + System.Math.Round(beam.Stress.Calculate(0), 4) + " MPa";
                infoitem.Items.Add(leftside);

                var rightside = new TreeViewItem();
                rightside.Header = GetString("rightside") + " : " + System.Math.Round(beam.Stress.Calculate(beam.Length), 4) + " MPa";
                infoitem.Items.Add(rightside);

                var maxvalue = new TreeViewItem();
                maxvalue.Header = GetString("maxvalue") + " : " + System.Math.Round(beam.Stress.YMax, 4) + " MPa";
                infoitem.Items.Add(maxvalue);
                var maxloc = new TreeViewItem();
                maxloc.Header = GetString("maxloc") + " : " + System.Math.Round(beam.Stress.YMaxPosition, 4) + " m";
                infoitem.Items.Add(maxloc);

                var minvalue = new TreeViewItem();
                minvalue.Header = GetString("minvalue") + " : " + System.Math.Round(beam.Stress.YMin, 4) + " MPa";
                infoitem.Items.Add(minvalue);
                var minloc = new TreeViewItem();
                minloc.Header = GetString("minloc") + " : " + System.Math.Round(beam.Stress.YMinPosition, 4) + " m";
                infoitem.Items.Add(minloc);

                var exploreritem = new TreeViewItem();
                var stressexplorer = new StressExplorer();
                stressexplorer.xvalue.Text = "0";
                stressexplorer.funcvalue.Text = System.Math.Round(beam.Stress.Calculate(0), 4).ToString();
                stressexplorer.xvalue.TextChanged += _mw.stressexplorer_TextChanged;
                exploreritem.Header = stressexplorer;
                infoitem.Items.Add(exploreritem);
            }
        }

        /// <summary>
        /// Removes TreeViewBeamItem from beam tree
        /// </summary>
        /// <param name="beam">The beam of the TreeViewBeamItem.</param>
        public void RemoveBeamTree(Beam beam)
        {
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (item.Beam.Equals(beam))
                {
                    _beamtree.Items.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates all the tree view items.
        /// </summary>
        public void UpdateAllBeamTree()
        {
            MesnetDebug.WriteInformation("Update All Tree Started");

            foreach (var item in Objects)
            {
                switch (GetObjectType(item.Value))
                {
                    case Global.ObjectType.Beam:
                        Beam beam = item.Value as Beam;
                        UpdateBeamTree(beam);
                        break;
                }
            }
        }

        public void BeamTreeItemSelected(object sender, RoutedEventArgs e)
        {
            if (BeamTreeItemSelectedEventEnabled)
            {
                _mw.Reset();

                var treeitem = sender as TreeViewItem;
                var beam = (treeitem.Header as BeamItem).Beam;

                SelectBeamItem(beam);

                foreach (var item in Objects)
                {
                    switch (GetObjectType(item.Value))
                    {
                        case Global.ObjectType.Beam:

                            var beam1 = item.Value as Beam;

                            if (Equals(beam1, beam))
                            {
                                _mw.SelectBeam(beam1);
                                return;
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Selects the beam item corresponds to given beam in beam tree.
        /// </summary>
        /// <param name="beam">The beam.</param>
        public void SelectBeamItem(Beam beam)
        {
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                if (Equals(beam, item.Beam))
                {
                    BeamTreeItemSelectedEventEnabled = false;
                    item.IsSelected = true;
                    BeamTreeItemSelectedEventEnabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Unselects all beam items in beam tree.
        /// </summary>
        public void UnSelectAllBeamItem()
        {
            MesnetDebug.WriteInformation("UnSelectAllBeamItem executed");
            foreach (TreeViewBeamItem item in _beamtree.Items)
            {
                item.Selected -= BeamTreeItemSelected;
                item.IsSelected = false;
                item.Selected += BeamTreeItemSelected;
            }
        }

#endregion

#region Support Tree Methods and Events

        /// <summary>
        /// Updates given support in the support tree view.
        /// </summary>
        /// <param name="support">The support.</param>
        public void UpdateSupportTree(object support)
        {
            var supportitem = new TreeViewSupportItem(support);
            bool exists = false;

            switch (GetObjectType(support))
            {
                case Global.ObjectType.SlidingSupport:

                    var slidingsup = support as SlidingSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == Global.ObjectType.SlidingSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new SlidingSupportItem(slidingsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                            new SlidingSupportItem(slidingsup);
                    }

#if DEBUG
                    var slideitem = new TreeViewItem();
                    slideitem.Header = "Id : " + slidingsup.Id;
                    supportitem.Items.Add(slideitem);
#endif

                    if (slidingsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + slidingsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var slmembersitem = new TreeViewItem();
                    slmembersitem.Header = GetString("connections");
                    supportitem.Items.Add(slmembersitem);

                    foreach (Member member in slidingsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var ssbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = ssbeamitem;

                        slmembersitem.Items.Add(memberitem);
                    }

                    break;

                case Global.ObjectType.BasicSupport:

                    var basicsup = support as BasicSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == Global.ObjectType.BasicSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;                      
                    }
                    else
                    {
                        supportitem.Header = new BasicSupportItem(basicsup);
                    }

#if DEBUG
                    var bsideitem = new TreeViewItem();
                    bsideitem.Header = "Id : " + basicsup.Id;
                    supportitem.Items.Add(bsideitem);
#endif

                    if (basicsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + basicsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var bsmembersitem = new TreeViewItem();
                    bsmembersitem.Header = GetString("connections");
                    supportitem.Items.Add(bsmembersitem);

                    foreach (Member member in basicsup.Members)
                    {
                        var memberitem = new TreeViewItem();
                        var bsbeamitem = new SupportBeamItem(member.Beam.BeamId, member.Direction, member.Moment);
                        memberitem.Header = bsbeamitem;
                        bsmembersitem.Items.Add(memberitem);
                    }

                    break;

                case Global.ObjectType.LeftFixedSupport:

                    var lfixedsup = support as LeftFixedSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == Global.ObjectType.LeftFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                            new LeftFixedSupportItem(lfixedsup);
                    }

#if DEBUG
                    var lsideitem = new TreeViewItem();
                    lsideitem.Header = "Id : " + lfixedsup.Id;
                    supportitem.Items.Add(lsideitem);
#endif

                    if (lfixedsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + lfixedsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var lfmembersitem = new TreeViewItem();
                    lfmembersitem.Header = GetString("connection");
                    supportitem.Items.Add(lfmembersitem);

                    var lfmemberitem = new TreeViewItem();

                    var lfbeamitem = new SupportBeamItem(lfixedsup.Member.Beam.BeamId, lfixedsup.Member.Direction,
                        lfixedsup.Member.Moment);

                    lfmemberitem.Header = lfbeamitem;

                    lfmembersitem.Items.Add(lfmemberitem);

                    break;

                case Global.ObjectType.RightFixedSupport:

                    var rfixedsup = support as RightFixedSupport;

                    foreach (TreeViewSupportItem item in _supporttree.Items)
                    {
                        if (GetObjectType(item.Support) == Global.ObjectType.RightFixedSupport)
                        {
                            if (Equals(supportitem.Support, item.Support))
                            {
                                item.Items.Clear();
                                supportitem = item;
                                exists = true;
                                break;
                            }
                        }

                    }

                    if (!exists)
                    {
                        supportitem.Header =
                            new RightFixedSupportItem(rfixedsup);
                        _supporttree.Items.Add(supportitem);
                        supportitem.Selected += SupportTreeItemSelected;
                    }
                    else
                    {
                        supportitem.Header =
                           new RightFixedSupportItem(rfixedsup);
                    }

#if DEBUG
                    var rsideitem = new TreeViewItem();
                    rsideitem.Header = "Id : " + rfixedsup.Id;
                    supportitem.Items.Add(rsideitem);
#endif

                    if (rfixedsup.CrossIndex != null)
                    {
                        var crossitem = new TreeViewItem();
                        crossitem.Header = GetString("crossindex") + " : " + rfixedsup.CrossIndex;
                        supportitem.Items.Add(crossitem);
                    }

                    var rfmembersitem = new TreeViewItem();
                    rfmembersitem.Header = GetString("connection");
                    supportitem.Items.Add(rfmembersitem);

                    var rfmemberitem = new TreeViewItem();

                    var rfbeamitem = new SupportBeamItem(rfixedsup.Member.Beam.BeamId, rfixedsup.Member.Direction,
                        rfixedsup.Member.Moment);

                    rfmemberitem.Header = rfbeamitem;

                    rfmembersitem.Items.Add(rfmemberitem);

                    break;
            }
        }

        /// <summary>
        /// Removes TreeViewSupportItem from support tree.
        /// </summary>
        /// <param name="support">The support of the TreeViewSupportItem.</param>
        public void RemoveSupportTree(object support)
        {
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                if (item.Support.Equals(support))
                {
                    _supporttree.Items.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates all the support tree view items.
        /// </summary>
        public void UpdateAllSupportTree()
        {
            MesnetDebug.WriteInformation("Update All Support Tree Started");
            foreach (var item in Objects)
            {
                switch (GetObjectType(item.Value))
                {
                    case Global.ObjectType.SlidingSupport:

                        var sliding = item.Value as SlidingSupport;
                        UpdateSupportTree(sliding);

                        break;

                    case Global.ObjectType.BasicSupport:

                        var basic = item.Value as BasicSupport;
                        UpdateSupportTree(basic);

                        break;

                    case Global.ObjectType.LeftFixedSupport:

                        var left = item.Value as LeftFixedSupport;
                        UpdateSupportTree(left);

                        break;

                    case Global.ObjectType.RightFixedSupport:

                        var right = item.Value as RightFixedSupport;
                        UpdateSupportTree(right);

                        break;
                }
            }
        }

        /// <summary>
        /// Selects the support item corresponds to given support in support tree.
        /// </summary>
        /// <param name="support">The support.</param>
        public void SelectSupportItem(object support)
        {
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                if (Equals(support, item.Support))
                {
                    SupportTreeItemSelectedEventEnabled = false;
                    item.IsSelected = true;
                    SupportTreeItemSelectedEventEnabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Unselects all support items in support tree.
        /// </summary>
        public void UnSelectAllSupportItem()
        {
            MesnetDebug.WriteInformation("UnSelectAllSupportItem executed");
            foreach (TreeViewSupportItem item in _supporttree.Items)
            {
                item.Selected -= SupportTreeItemSelected;
                item.IsSelected = false;
                item.Selected += SupportTreeItemSelected;
            }
        }

        /// <summary>
        /// Executed when any support item selected in the treeview. It selects and highlights corresponding support.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SupportTreeItemSelected(object sender, RoutedEventArgs e)
        {
            if (SupportTreeItemSelectedEventEnabled)
            {
                MesnetDebug.WriteInformation("SupportTreeItemSelected");
                _mw.Reset();
                var treeitem = sender as TreeViewItem;

                if (treeitem.Header is SlidingSupportItem ssitem)
                {
                    foreach (var item in Objects)
                    {
                        switch (GetObjectType(item.Value))
                        {
                            case Global.ObjectType.SlidingSupport:

                                var slidingsupport = item.Value as SlidingSupport;

                                if (Equals(ssitem.Support, slidingsupport))
                                {
                                    slidingsupport.Select();
                                    _mw.selectesupport = slidingsupport;
                                    SelectSupportItem(slidingsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is BasicSupportItem bsitem)
                {
                    foreach (var item in Objects)
                    {
                        switch (GetObjectType(item.Value))
                        {
                            case Global.ObjectType.BasicSupport:

                                var basicsupport = item.Value as BasicSupport;

                                if (Equals(bsitem.Support, basicsupport))
                                {
                                    basicsupport.Select();
                                    _mw.selectesupport = basicsupport;
                                    SelectSupportItem(basicsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is LeftFixedSupportItem lsitem)
                {
                    foreach (var item in Objects)
                    {
                        switch (GetObjectType(item.Value))
                        {
                            case Global.ObjectType.LeftFixedSupport:

                                var leftfixedsupport = item.Value as LeftFixedSupport;

                                if (Equals(lsitem.Support, leftfixedsupport))
                                {
                                    leftfixedsupport.Select();
                                    _mw.selectesupport = leftfixedsupport;
                                    SelectSupportItem(leftfixedsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
                else if (treeitem.Header is RightFixedSupportItem rsitem)
                {
                    foreach (var item in Objects)
                    {
                        switch (GetObjectType(item.Value))
                        {
                            case Global.ObjectType.RightFixedSupport:

                                var rightfixedsupport = item.Value as RightFixedSupport;

                                if (Equals(rsitem.Support, rightfixedsupport))
                                {
                                    rightfixedsupport.Select();
                                    _mw.selectesupport = rightfixedsupport;
                                    SelectSupportItem(rightfixedsupport);
                                    return;
                                }

                                break;
                        }
                    }
                }
            }
        }
#endregion
    }
}
